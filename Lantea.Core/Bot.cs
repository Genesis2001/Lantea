// -----------------------------------------------------------------------------
//  <copyright file="Bot.cs" company="Zack Loveless">
//      Copyright (c) Zack Loveless.  All rights reserved.
//  </copyright>
// -----------------------------------------------------------------------------

namespace Lantea.Core
{
	using System;
	using System.Collections.Generic;
	using System.ComponentModel.Composition.Hosting;
	using System.IO;
	using System.Reflection;
	using System.Text;
	using Common.IO;
	using IO;
	using Modules;
	using Net.Irc;

	public class Bot : IDisposable
	{
		private IEnumerable<Lazy<IModule, IModuleMeta>> modules;
		private ISettingsManager settings;

		public string Nick { get; private set; }

		public string RealName { get; set; }

		public ILog Log { get; private set; }

		public IrcClient Client { get; private set; }

		public void Compose()
		{
			var container = GetCompositionContainer();

			// modules       = container.GetExports<IModule, IModuleMeta>();
		}

		private static CompositionContainer GetCompositionContainer()
		{
			var asm                = Assembly.GetEntryAssembly();
			var loc                = Path.GetDirectoryName(asm.Location);
			var extensionDirectory = Path.Combine(loc, "Extensions");

			if (!Directory.Exists(extensionDirectory))
			{
				Directory.CreateDirectory(extensionDirectory);
			}

			var catalog = new AggregateCatalog();
			catalog.Catalogs.Add(new DirectoryCatalog(extensionDirectory));

			return new CompositionContainer(catalog);
		}

		private void RegisterClientEvents()
		{
			Client.RawMessageEvent            += OnRawMessageReceived;
			Client.TimeoutEvent               += OnClientTimeout;
			Client.ConnectionEstablishedEvent += OnClientConnect;

#if DEBUG
			Client.RfcNumericEvent      += OnRfcNumericReceived;
			Client.ChannelJoinEvent     += OnChannelJoin;
			Client.ChannelPartEvent     += OnChannelPart;
			Client.MessageReceivedEvent += OnMessageReceived;
			Client.NoticeReceivedEvent  += OnNoticeReceived;
			Client.PingReceiptEvent     += OnPingReceipt;
#endif
		}

		private void OnClientConnect(object sender, EventArgs eventArgs)
		{
			if (Log != null) Log.Info("Connection established to server.");
		}

#if DEBUG
		private void OnPingReceipt(object sender, EventArgs args)
		{
			if (Log != null) Log.Debug("Ping received. Sent pong.");
		}

		private void OnNoticeReceived(object sender, MessageReceivedEventArgs args)
		{
			if (Log != null) Log.DebugFormat("[MSG] Received notice from {0} (to: {1}) sent: {2}", args.Nick, args.Target, args.Message);
		}

		private void OnMessageReceived(object sender, MessageReceivedEventArgs args)
		{
			if (Log != null) Log.DebugFormat("[MSG] Received message from {0} (to: {1}) sent: {2}", args.Nick, args.Target, args.Message);
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

		private void OnMessageSend(object sender, RawMessageEventArgs args)
		{
			if (Log != null) Log.DebugFormat("SEND: {0}", args.Message);
		}

		private void OnRfcNumericReceived(object sender, RfcNumericEventArgs args)
		{
			if (args.Numeric.Equals(001))
			{
				Log.Info("Bot started.");
				Client.Send("JOIN #test");

				// Not yet implemented, but meh.
				/*var perform = settings.GetValues("/Settings/Connection/Events/OnConnect/Execute/@Command");
				foreach (var item in perform)
				{
					client.Send(item);
				}*/
			}
		}

		private void OnRawMessageReceived(object sender, RawMessageEventArgs args)
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

			Client.My.RealName         = RealName;
			Client.Host                = settings.GetValue("/Settings/Connection/@Host");
			Client.Encoding            = IrcEncoding.UTF8;
			
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
