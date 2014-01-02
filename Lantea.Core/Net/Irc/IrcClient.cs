// -----------------------------------------------------------------------------
//  <copyright file="IrcClient.cs" company="Zack Loveless">
//      Copyright (c) Zack Loveless.  All rights reserved.
//  </copyright>
// -----------------------------------------------------------------------------

namespace Lantea.Core.Net.Irc
{
	using System;
	using System.Collections.Generic;
	using System.Linq;
	using System.Net;
	using System.Net.Sockets;
	using System.Text;
	using System.Threading;
	using System.Threading.Tasks;
	using Common.Collections;
	using Common.Collections.Concurrent;
	using Common.Linq;
	using Common.Net;
	using Data;

	public partial class IrcClient : IDisposable
	{
		[Flags]
		public enum ConnectOptions
		{
			Default = 0,
			Secure  = 1,
		}

		// ReSharper disable FieldCanBeMadeReadOnly.Local
		private readonly IQueue<string> messageQueue;
		private ITcpClientAsync client;
		
		private Task queueRunner;
		
		private readonly CancellationTokenSource tokenSource;
		private CancellationToken token;
		// ReSharper restore FieldCanBeMadeReadOnly.Local

		public IrcClient(string nick)
		{
			tokenSource             = new CancellationTokenSource();
			token                   = tokenSource.Token;

			Options                 = ConnectOptions.Default;
			messageQueue            = new ConcurrentQueueAdapter<string>();
			My                      = new User(nick);
			Timeout                 = TimeSpan.FromMinutes(10d);
			QueueInteval            = 1000;
			Channels                = new HashSet<Channel>();

			RawMessageEvent        += RegistrationHandler;
			RawMessageEvent        += RfcNumericHandler;
			RawMessageEvent        += PingHandler;
			RawMessageEvent        += JoinPartHandler;
			RawMessageEvent        += MessageNoticeHandler;

			token.Register(CancellationNoticeHandler);
			My.SetClient(this);
		}

		#region Properties

		/// <summary>
		/// Gets a <see cref="T:System.Boolean" /> value representing whether the <see cref="T:IrcClient" /> has connected to the Host.
		/// </summary>
		public bool Connected
		{
			get { return client != null && client.Connected; }
		}

		public HashSet<Channel> Channels { get; private set; }

		/// <summary>
		/// Gets or sets a value representing what type of encoding to use for encoding messages sent to the Host.
		/// </summary>
		public IrcEncoding Encoding { get; set; }

		/// <summary>
		/// Gets or sets a <see cref="T:System.String" /> value representing the name of the Host in which the <see cref="T:IrcClient" /> will be connecting.
		/// </summary>
		public string Host { get; set; }

		/// <summary>
		/// Gets a <see cref="T:System.Boolean" /> value representing whether the <see cref="T:IrcClient" /> has been initialized.
		/// </summary>
		public bool IsInitialized
		{
			get
			{
				var val = true;

				if (string.IsNullOrEmpty(My.Nick)) val = false;
				else if (string.IsNullOrEmpty(Host) || Dns.GetHostEntry(Host) == null) val = false;
				else if (Port <= 0) val = false;

				return val;
			}
		}

		/// <summary>
		/// Gets an IRC User reference to the <see cref="T:IrcClient" />'s user entity.
		/// </summary>
		public User My { get; private set; }

		/// <summary>
		/// Gets or sets a <see cref="T:System.String" /> value representing the password to be used for protocol registration.
		/// </summary>
		public string Password { get; set; }

		/// <summary>
		/// Gets or sets a <see cref="T:System.Int32" /> value representing the port in which the <see cref="T:IrcClient" /> will be connecting.
		/// </summary>
		public int Port { get; set; }

		/// <summary>
		/// Gets or sets a <see cref="T:System.Int32" /> value representing the 
		/// </summary>
		public int QueueInteval { get; set; }

		

		/// <summary>
		/// Gets or sets a value representing the timeout interval for messages being received.
		/// </summary>
		public TimeSpan Timeout { get; set; }

		/// <summary>
		/// Gets or sets a bit-mask value representing the options for connecting to the Host.
		/// </summary>
		public ConnectOptions Options { get; set; }

		#endregion

		#region Events

		public event EventHandler ConnectionEstablishedEvent;
		public event EventHandler<JoinPartEventArgs> ChannelJoinEvent;
		public event EventHandler<JoinPartEventArgs> ChannelPartEvent;
		public event EventHandler<MessageReceivedEventArgs> MessageReceivedEvent;
		public event EventHandler<MessageReceivedEventArgs> NoticeReceivedEvent;
		public event EventHandler PingReceiptEvent;
		public event EventHandler<RawMessageEventArgs> RawMessageEvent;
		public event EventHandler<RawMessageEventArgs> RawMessageTransmitEvent;
		public event EventHandler<RfcNumericEventArgs> RfcNumericEvent;
		public event EventHandler TimeoutEvent;

		#endregion

		#region Methods

		private void ChangeNick(string nick)
		{
			Send("NICK {0}", My.Nick);
		}

		public Channel GetChannel(string channelName)
		{
			if (string.IsNullOrEmpty(channelName))
				throw new ArgumentNullException("channelName", "The channelName parameter cannot be null or empty.");

			var c = Channels.SingleOrDefault(x => x.Name.EqualsIgnoreCase(channelName));

			if (c == null)
			{
				c = new Channel(channelName);
				c.SetClient(this);

				Channels.Add(c);
			}

			return c;
		}

		private void SetDefaults()
		{
			if (string.IsNullOrEmpty(My.Ident)) My.Ident = My.Nick.ToLower();
		}

		/// <summary>
		/// Starts the <see cref="T:IrcClient" /> process of reading and writing to the Host.
		/// </summary>
		public void Start()
		{
			if (!IsInitialized)
			{
				throw new InvalidOperationException(
					string.Format(
					              "Unable to start the current {0} as it as not been properly initialized. Please refer to the documentation.",
						GetType().Name));
			}

			Encoding encoding;
			switch (Encoding)
			{
				default:
					encoding = new ASCIIEncoding();
					break;

				case IrcEncoding.UTF8:
					encoding = new UTF8Encoding(false);
					break;
			}

			SetDefaults();

			client = new TcpClientAsyncAdapter(new TcpClient(), encoding);
			try
			{
				client.Connect(Host, Port);
			}
			catch (ArgumentNullException)
			{
				tokenSource.Cancel();
			}

			client.ReadLineAsync().ContinueWith(OnAsyncRead, token);
			queueRunner = Task.Run(new Action(QueueHandler), token);

			TickTimeout();
		}
		
		private void Send(string data)
		{
			client.WriteLine(data);

			RawMessageTransmitEvent.Raise(this, new RawMessageEventArgs(data));
		}

		/// <summary>
		/// Enqueues the specified formatted <see cref="T:System.String" />  value to be sent to the Host.
		/// </summary>
		/// <param name="format"></param>
		/// <param name="args"></param>
		public void Send(string format, params object[] args)
		{
			var sb = new StringBuilder();
			sb.AppendFormat(format, args);

			messageQueue.Push(sb.ToString());
		}

		#endregion

		#region Implementation of IDisposable

		/// <summary>
		/// Performs application-defined tasks associated with freeing, releasing, or resetting unmanaged resources.
		/// </summary>
		public void Dispose()
		{
			Dispose(true);
			GC.SuppressFinalize(this);
		}

		protected virtual void Dispose(bool disposing)
		{
			if (!disposing) return;

			tokenSource.Cancel();
		}

		#endregion
	}
}
