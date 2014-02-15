// -----------------------------------------------------------------------------
//  <copyright file="Bot.cs" company="Zack Loveless">
//      Copyright (c) Zack Loveless.  All rights reserved.
//  </copyright>
// -----------------------------------------------------------------------------

namespace Lantea.Core
{
	using System;
	using System.Collections.Generic;
	using System.IO;
	using Atlantis.Net.Irc;
	using IO;
	using Modules;

	public class Bot : IDisposable
	{
		private IEnumerable<Lazy<IModule, IModuleMeta>> modules;
		private ISettingsManager settings;

		public string Nick { get; private set; }

		public string RealName { get; set; }

		public ILog Log { get; private set; }

		public IrcClient Client { get; private set; }
		
		private void RegisterClientEvents()
		{
			Client.TimeoutEvent               += OnClientTimeout;
			Client.ConnectionEstablishedEvent += OnClientConnect;

#if DEBUG
			Client.FillListsOnJoin       = true;
			Client.FillListsDelay        = 10;

			Client.RawMessageReceivedEvent += OnRawMessageReceivedReceived;
			/*Client.ChannelJoinEvent        += OnChannelJoin;
			Client.ChannelPartEvent        += OnChannelPart;*/
			Client.MessageReceivedEvent    += OnMessageReceived;
			/*Client.NickChangedEvent        += OnNickChanged;
			Client.NoticeReceivedEvent     += OnNoticeReceived;
			Client.PingReceiptEvent        += OnPingReceipt;
			Client.QuitEvent               += OnQuit;*/
#endif
		}

		private void OnQuit(object sender, QuitEventArgs e)
		{
			if (Log != null) Log.InfoFormat("[QUIT] {0} has left the IRC server: {1}", e.Nick, e.Message);
		}

		private void OnClientConnect(object sender, EventArgs args)
		{
			if (Log != null)
			{
				Log.Info("Connection established to server.");
				Log.Info("Bot started.");
			}

			Client.Send("JOIN #UnifiedTech");
		}

#if DEBUG
		private void OnNickChanged(object sender, NickChangeEventArgs args)
		{
			if (Log != null) Log.DebugFormat("{0} changed nicks to {1}", args.OldNick, args.NewNick);
		}

		private void OnPingReceipt(object sender, EventArgs args)
		{
			if (Log != null) Log.Debug("Ping received. Sent pong.");
		}

		private void OnNoticeReceived(object sender, MessageReceivedEventArgs args)
		{
			if (Log != null) Log.DebugFormat("[MSG] Received notice from {0} (to: {1}) sent: {2}", args.Source, args.Target, args.Message);
		}

		private void OnMessageReceived(object sender, MessageReceivedEventArgs args)
		{
			//if (Log != null) Log.DebugFormat("[MSG] Received message from {0} (to: {1}) sent: {2}", args.Source, args.Target, args.Message);

			if (args.Message.StartsWith("!perm"))
			{
				var c = Client.GetChannel(args.Target);
				var n = c.Users[args.Source];

				Client.Message(c.Name, "{0}, you currently have {1} as your highest prefix.", args.Source, n.HighestPrefix);
			}
		}

		private void OnChannelJoin(object sender, JoinPartEventArgs args)
		{
			if (Log != null) Log.DebugFormat("[JOIN] {0} joined {1}", args.Nick, args.Channel);
		}

		private void OnChannelPart(object sender, JoinPartEventArgs args)
		{
			if (Log != null) Log.DebugFormat("[PART] {0} left {1}", args.Nick, args.Channel);
		}
#endif

		private void OnClientTimeout(object sender, EventArgs eventArgs)
		{
			// TODO: /Settings/Connection/@Retry - add reconnection attempts upon timeout.
			if (Log != null) Log.Warn("Timeout detected. Exiting.");

			Environment.Exit(1);
		}

		private void OnRawMessageReceivedReceived(object sender, RawMessageEventArgs args)
		{
			if (Log != null) Log.DebugFormat("RECV: {0}", args.Message);
		}

		public void LoadSettings(string config)
		{
			settings = new SettingsManager(config);
			settings.Load();

#if DEBUG
			Log = new LogWriter<TextWriter>(Console.Out) { PrefixLog = false, Threshold = LogThreshold.Verbose };
#else
			Log = new LogStream<FileStream>(new FileStream("lantea.log", FileMode.Append, FileAccess.Write, FileShare.Read))
			      {
				      Encoding  = Encoding.UTF8,
				      Threshold = LogThreshold.Error,
				      PrefixLog = true,
			      };
#endif
		}

		public void Start()
		{
			Log.Info("Bot starting.");

			Nick     = settings.GetValue("/Settings/Connection/@Nick");
			RealName = settings.GetValue("/Settings/Connection/@RealName");
			Client   = new IrcClient(Nick);

			int port;
			if (Int32.TryParse(settings.GetValue("/Settings/Connection/@Port"), out port))
			{
				Client.Port = port;
			}

			//var foo = settings.GetValues("/Settings/Connection/Options/Secure[@CertificatePath and @CertificateKeyPath]");

			Client.RealName = RealName;
			Client.Host     = settings.GetValue("/Settings/Connection/@Host");
			Client.Encoding = IrcEncoding.UTF8;
			
			RegisterClientEvents();

			Client.Start();
		}

		#region Implementation of IDisposable

		/// <summary>
		///     Performs application-defined tasks associated with freeing, releasing, or resetting unmanaged resources.
		/// </summary>
		public void Dispose()
		{
			Dispose(true);
			GC.SuppressFinalize(this);
		}

		protected virtual void Dispose(bool disposing)
		{
			if (disposing)
			{
				Log.Dispose();
				Client.Dispose();
			}
		}

		#endregion
	}
}
