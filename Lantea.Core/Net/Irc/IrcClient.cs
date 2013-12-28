// -----------------------------------------------------------------------------
//  <copyright file="IrcClient.cs" company="Zack Loveless">
//      Copyright (c) Zack Loveless.  All rights reserved.
//  </copyright>
// -----------------------------------------------------------------------------

namespace Lantea.Core.Net.Irc
{
	using System;
	using System.Net;
	using System.Net.Sockets;
	using System.Text;
	using System.Threading;
	using System.Threading.Tasks;
	using System.Timers;
	using Common.Collections;
	using Common.Collections.Concurrent;
	using Common.Linq;
	using Common.Net;
	using Data;
	using Timer = System.Timers.Timer;

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

		public IrcClient(string nick, string realName)
		{
			tokenSource             = new CancellationTokenSource();
			token                   = tokenSource.Token;

			Options                 = ConnectOptions.Default;
			messageQueue            = new ConcurrentQueueAdapter<string>();
			User                    = new User(this, nick, realName);
			Timeout                 = TimeSpan.FromMinutes(10d);
			QueueInteval            = 1000;

			RawMessageEvent        += RegistrationHandler;
			RawMessageEvent        += RfcNumericHandler;
			RawMessageEvent        += PingHandler;

			token.Register(CancellationNoticeHandler);
		}

		#region Properties

		public bool Connected
		{
			get { return client != null && client.Connected; }
		}

		public IrcEncoding Encoding { get; set; }

		public string Host { get; set; }

		public User User { get; private set; }

		public string Password { get; set; }

		public int Port { get; set; }

		public int QueueInteval { get; set; }

		public bool IsInitialized
		{
			get
			{
				var val = true;

				if (string.IsNullOrEmpty(User.Nick)) val = false;
				else if (string.IsNullOrEmpty(Host) || Dns.GetHostEntry(Host) == null) val = false;

				return val;
			}
		}

		public TimeSpan Timeout { get; set; }

		public ConnectOptions Options { get; set; }

		#endregion

		#region Events

		public event EventHandler<RfcNumericEventArgs> RfcNumericEvent;
		public event EventHandler<RawMessageEventArgs> RawMessageEvent;
		public event EventHandler<RawMessageEventArgs> RawMessageTransmitEvent;
		public event EventHandler TimeoutEvent;

		#endregion

		#region Methods

		private void SetDefaults()
		{
			if (string.IsNullOrEmpty(User.Ident)) User.Ident = User.Nick.ToLower();
		}

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
				// TODO: Raise disconnection event (eventually)
			}

			client.ReadLineAsync().ContinueWith(OnAsyncRead, token);
			queueRunner           = Task.Run(new Action(QueueHandler), token);

			timeoutTimer          = new Timer(Timeout.TotalMilliseconds);
			timeoutTimer.Elapsed += TimeoutTimerElapsed;
			timeoutTimer.Start();
		}
		
		private void Send(string data)
		{
			client.WriteLine(data);

			RawMessageTransmitEvent.Raise(this, new RawMessageEventArgs(data));
		}

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
