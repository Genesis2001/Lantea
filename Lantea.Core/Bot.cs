// -----------------------------------------------------------------------------
//  <copyright file="Bot.cs" company="Zack Loveless">
//      Copyright (c) Zack Loveless.  All rights reserved.
//  </copyright>
// -----------------------------------------------------------------------------

namespace Lantea.Core
{
	using System;
	using System.Collections.Generic;
	using Common.IO;
	using Common.Modules;
	using IO;
	using Net.Irc;

	public class Bot : IDisposable
	{
		private IrcClient client;
		private IEnumerable<IModule> modules;
		private ISettingsManager settings;

		public Bot()
		{
			Log = new Log("lantea.log") {PrefixLog = true};
		}

		public string Nick { get; private set; }

		public string RealName { get; set; }

		public ILog Log { get; private set; }

		private void Compose()
		{
		}

		private void RegisterEvents()
		{
			client.RawMessageEvent += RawMessageEventCallback;
			client.RfcNumericEvent += RfcNumericEventCallback;
		}

		private void RfcNumericEventCallback(object sender, RfcNumericEventArgs args)
		{
			// 
		}

		private void RawMessageEventCallback(object sender, RawMessageEventArgs args)
		{
			if (Log != null) Log.Debug(args.Message);
		}

		public void LoadSettings(string configFile)
		{
			settings = new SettingsManager(configFile);
			settings.Load();
		}

		public void Start()
		{
			Nick     = settings.GetValue("");
			RealName = settings.GetValue("");
			client   = new IrcClient(Nick, RealName);

			RegisterEvents();
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
				// TODO: client.Disconnect()
			}
		}

		#endregion
	}
}
