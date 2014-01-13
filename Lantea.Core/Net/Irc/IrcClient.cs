﻿// -----------------------------------------------------------------------------
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

		private CancellationTokenSource queueTokenSource;
		private readonly CancellationTokenSource tokenSource;
		private CancellationToken token;
		private CancellationToken queueToken;
		private bool enableFakeLag;
		// ReSharper restore FieldCanBeMadeReadOnly.Local

		public IrcClient(string nick)
		{
			tokenSource             = new CancellationTokenSource();
			token                   = tokenSource.Token;

			Channels                = new HashSet<Channel>();
			messageQueue            = new ConcurrentQueueAdapter<string>();
			Nick                    = nick;
			Options                 = ConnectOptions.Default;
			QueueInteval            = 1000;
			EnableFakeLag           = true;
			RetryInterval           = TimeSpan.FromMinutes(5d).TotalMilliseconds;
			Timeout                 = TimeSpan.FromMinutes(10d);
			Modes                   = new List<char>();

			RawMessageEvent        += RegistrationHandler;
			RawMessageEvent        += RfcNumericHandler;
			RawMessageEvent        += PingHandler;
			RawMessageEvent        += JoinPartHandler;
			RawMessageEvent        += MessageNoticeHandler;
			RawMessageEvent        += NickHandler;

			RfcNumericEvent        += ConnectionHandler;
			RfcNumericEvent        += ProtocolHandler;
			RfcNumericEvent        += ChannelAccessHandler;
			RfcNumericEvent        += NickInUseHandler;

			token.Register(CancellationNoticeHandler);
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

		public bool EnableFakeLag
		{
			get { return enableFakeLag; }
			set
			{
				enableFakeLag = value;

				if (value)
				{
					queueTokenSource.Cancel();
				}
				else StartQueue();
			}
		}

		/// <summary>
		/// Gets or sets a value representing what type of encoding to use for encoding messages sent to the Host.
		/// </summary>
		public IrcEncoding Encoding { get; set; }

		/// <summary>
		/// Gets or sets a <see cref="T:System.String" /> value representing the name of the Host in which the <see cref="T:IrcClient" /> will be connecting.
		/// </summary>
		public string Host { get; set; }

		/// <summary>
		/// Gets or sets a <see cref="T:System.String" /> value representing the Ident part for the <see cref="T:IrcClient" />'s user.
		/// </summary>
		public string Ident { get; set; }

		/// <summary>
		/// Gets a <see cref="T:System.Boolean" /> value representing whether the <see cref="T:IrcClient" /> has been initialized.
		/// </summary>
		public bool IsInitialized
		{
			get
			{
				var val = true;

				if (string.IsNullOrEmpty(Nick)) val = false;
				else if (string.IsNullOrEmpty(Host) || Dns.GetHostEntry(Host) == null) val = false;
				else if (Port <= 0) val = false;

				return val;
			}
		}

		/// <summary>
		/// Gets a list of modes that are currently set on the client.
		/// </summary>
		public List<char> Modes { get; private set; }

		/// <summary>
		/// Gets or sets a <see cref="T:System.String" /> value representing the nickname for the <see cref="T:IrcClient" /> 
		/// </summary>
		public string Nick { get; set; }

		/// <summary>
		/// Gets or sets a bit-mask value representing the options for connecting to the Host.
		/// </summary>
		public ConnectOptions Options { get; set; }

		/// <summary>
		/// Gets or sets a <see cref="T:System.String" /> value representing the password to be used for protocol registration.
		/// </summary>
		public string Password { get; set; }

		/// <summary>
		/// Gets or sets a <see cref="T:System.Int32" /> value representing the port in which the <see cref="T:IrcClient" /> will be connecting.
		/// </summary>
		public int Port { get; set; }

		/// <summary>
		/// Gets or sets a <see cref="T:System.Int32" /> value representing the interval for processing queued messages.
		/// </summary>
		public int QueueInteval { get; set; }

		/// <summary>
		/// Gets or sets a <see cref="T:System.String" /> value representing the <see cref="T:IrcClient" />'s real name.
		/// </summary>
		public string RealName { get; set; }

		/// <summary>
		/// Gets or sets a <see cref="T:System.Boolean" /> value indicating whether to retry connecting to the IRC server.
		/// </summary>
		public bool RetryConnect { get; set; }

		/// <summary>
		/// Gets or sets a <see cref="T:System.Boolean" /> value indicating whether to retry the client's primary nickname.
		/// </summary>
		public bool RetryNick { get; set; }

		/// <summary>
		/// Gets or sets a <see cref="T:System.Int32" /> value representing the inteval for retry attempts
		/// </summary>
		public double RetryInterval { get; set; }

		/// <summary>
		/// Gets or sets a <see cref="T:System.Boolean" /> value indicating whether to request RPL_NAMEREPLY from the server when someone joins/leaves a channel or changes modes on a channel.
		/// </summary>
		public bool StrictNames { get; set; }

		/// <summary>
		/// Gets or sets a value representing the timeout interval for messages being received.
		/// </summary>
		public TimeSpan Timeout { get; set; }

		#endregion

		#region Events

		public event EventHandler ConnectionEstablishedEvent;
		public event EventHandler<JoinPartEventArgs> ChannelJoinEvent;
		public event EventHandler<JoinPartEventArgs> ChannelPartEvent;
		public event EventHandler<MessageReceivedEventArgs> MessageReceivedEvent;
		public event EventHandler<NickChangeEventArgs> NickChangedEvent;
		public event EventHandler<MessageReceivedEventArgs> NoticeReceivedEvent;
		public event EventHandler PingReceiptEvent;
		public event EventHandler<RawMessageEventArgs> RawMessageEvent;
		public event EventHandler<RawMessageEventArgs> ClientSocketWriteEvent;
		public event EventHandler<RfcNumericEventArgs> RfcNumericEvent;
		public event EventHandler TimeoutEvent;

		#endregion

		#region Methods

		private void ChangeNick(string nick)
		{
			Send("NICK {0}", nick);
		}

		public Channel GetChannel(string channelName)
		{
			if (string.IsNullOrEmpty(channelName))
				throw new ArgumentNullException("channelName", "The channelName parameter cannot be null or empty.");

			var c = Channels.SingleOrDefault(x => x.Name.EqualsIgnoreCase(channelName));

			if (c == null)
			{
				c = new Channel(this, channelName);
				Channels.Add(c);
			}

			return c;
		}

		public void Message(string target, string format, params object[] args)
		{
			var message = string.Format(format, args);

			Send("PRIVMSG {0} :{1}", target, message);
		}

		public void Notice(string target, string format, params object[] args)
		{
			var message = string.Format(format, args);

			Send("NOTICE {0} :{1}", target, message);
		}

		private void SetDefaults()
		{
			if (string.IsNullOrEmpty(Ident)) Ident = Nick.ToLower();
			if (string.IsNullOrEmpty(RealName)) RealName = Nick;
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
				if (Options.HasFlag(ConnectOptions.Secure))
				{
					// TODO: Call client.ConnectSecurely(string host, int port, [something] certificatePfx);
					client.Connect(Host, Port);
				}
				else
				{
					client.Connect(Host, Port);
				}
			}
			catch (ArgumentNullException)
			{
				tokenSource.Cancel();
			}

			client.ReadLineAsync().ContinueWith(OnAsyncRead, token);

			StartQueue();
			TickTimeout();
		}

		private void StartQueue()
		{
			queueRunner = Task.Run(new Action(QueueProcessor), queueToken);
		}
		
		private void Send(string data)
		{
			client.WriteLine(data);

			ClientSocketWriteEvent.Raise(this, new RawMessageEventArgs(data));
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

			if (EnableFakeLag)
			{
				messageQueue.Push(sb.ToString());
			}
			else Send(sb.ToString());
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
