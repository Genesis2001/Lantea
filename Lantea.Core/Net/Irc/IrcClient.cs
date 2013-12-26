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
	using Common.Collections;
	using Common.Collections.Concurrent;
	using Common.Linq;
	using Common.Net;
	using Data;

	public partial class IrcClient : IDisposable
	{
		private readonly IQueue<string> messageQueue;
		private ITcpClient client;

		private Task workerTask;
		private Task queueRunner;

		/*private Thread queueThread;
		private Thread workerThread;*/

		private readonly CancellationTokenSource cancellationTokenSource;

		public IrcClient(string nickAlias, string realName)
		{
			cancellationTokenSource = new CancellationTokenSource();
			messageQueue = new ConcurrentQueueAdapter<string>();

			User = new User(this, nickAlias, realName);
			QueueInteval = 4000;
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

		#endregion

		#region Events

		public event EventHandler<RfcNumericEventArgs> RfcNumericEvent;
		public event EventHandler<RawMessageEventArgs> RawMessageEvent;

		#endregion

		#region Methods

		private string BuildRegistrationPacket()
		{
			var sb = new StringBuilder();

			if (!string.IsNullOrEmpty(Password))
			{
				sb.AppendFormat("PASS :{0}\n", Password);
			}

			sb.AppendFormat("NICK {0}\n", User.Nick);
			sb.AppendFormat("USER {0} 0 * :{1}\n", User.Ident, User.RealName);

			return sb.ToString();
		}

		public bool Start()
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

			client = new TcpClientAsyncAdapter(new TcpClient(), encoding);
			try
			{
				client.Connect(Host, Port);
			}
			catch (ArgumentNullException)
			{
				// TODO: Raise disconnection event (eventually)
			}

			workerTask = Task.Run(new Action(ThreadWorkerCallback), cancellationTokenSource.Token);

			throw new NotImplementedException();
		}
		
		private void Send(string data)
		{
			client.Write(data);
		}

		public void Send(string format, params object[] args)
		{
			var sb = new StringBuilder();
			sb.AppendLineFormat(format, args);

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

			cancellationTokenSource.Cancel();
		}

		#endregion
	}
}
