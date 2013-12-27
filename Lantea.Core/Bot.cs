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
	using Common.Modules;
	using IO;
	using Net.Irc;

	public class Bot : IDisposable
	{
		private IEnumerable<Lazy<IModule, IModuleMeta>> modules;
		private ISettingsManager settings;

		public string Nick { get; private set; }

		public string RealName { get; set; }

		public ILog Log { get; private set; }

		public IrcClient Client { get; private set; }

		private void Compose()
		{
			var container = GetCompositionContainer();

#if DEBUG
			Log = new LogWriter<TextWriter>(Console.Out) {PrefixLog = false, Threshold = LogThreshold.Verbose};
#else
			Log = new LogStream<FileStream>(new FileStream("lantea.log", FileMode.Append, FileAccess.Write, FileShare.Read))
			      {
				      Encoding  = Encoding.UTF8,
				      Threshold = LogThreshold.Error,
				      PrefixLog = true,
			      };
#endif

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
			Client.RawMessageTransmitEvent += DebugMessageTransmissionEventCallback;
			Client.RawMessageEvent         += RawMessageEventCallback;
			Client.RfcNumericEvent         += RfcNumericEventCallback;
			Client.TimeoutEvent            += ClientTimeOutCallback;
		}

		private void ClientTimeOutCallback(object sender, EventArgs eventArgs)
		{
			// TODO: /Settings/Connection/@Retry - add reconnection attempts upon timeout.
			if (Log != null) Log.Warn("Timeout detected. Exiting.");

			Environment.Exit(1);
		}

		private void DebugMessageTransmissionEventCallback(object sender, RawMessageEventArgs args)
		{
			if (Log != null) Log.DebugFormat("IN: {0}", args.Message);
		}

		private void RfcNumericEventCallback(object sender, RfcNumericEventArgs args)
		{
			if (Log != null) Log.DebugFormat("RECV: NUM({0}) MSG(\"{1}\")", args.Numeric, args.Message);

			if (args.Numeric.Equals(001))
			{
				// Not yet implemented, but meh.
				/*var perform = settings.GetValues("/Settings/Connection/Events/OnConnect/Execute/@Command");
				foreach (var item in perform)
				{
					client.Send(item);
				}*/
			}
		}

		private void RawMessageEventCallback(object sender, RawMessageEventArgs args)
		{
			if (Log != null) Log.DebugFormat("OUT: {0}", args.Message);
		}

		public void LoadSettings(string configFile)
		{
			settings = new SettingsManager(configFile);
			settings.Load();
		}

		public void Start()
		{
			Log.Info("Bot starting.");

			Nick     = settings.GetValue("/Settings/Connection/@Nick");
			RealName = settings.GetValue("/Settings/Connection/@RealName");
			Client   = new IrcClient(Nick, RealName);

			int port;
			if (Int32.TryParse(settings.GetValue("/Settings/Connection/@Port"), out port))
			{
				Client.Port = port;
			}

			//var foo = settings.GetValues("/Settings/Connection/Options/Secure[@CertificatePath and @CertificateKeyPath]");

			Client.Host         = settings.GetValue("/Settings/Connection/@Host");
			Client.Encoding     = IrcEncoding.UTF8;

			Compose();

			RegisterClientEvents();

			Client.Start();

			Log.Info("Bot started.");
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
