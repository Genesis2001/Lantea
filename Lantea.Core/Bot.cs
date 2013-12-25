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
	using IO;
	using Net.Irc;

	public class Bot : IDisposable
	{
		private IrcClient client;
		private IEnumerable<Object> modules; // TODO

		public Bot()
		{
			Log = new Log("lantea.log") {PrefixLog = true};
		}

		public ILog Log { get; private set; }

		private void Compose()
		{
			// 
		}

		private void Load()
		{
			// TODO: load configuration.
			// TODO: decide on a configuration structure.
		}

		public void Start()
		{
			// Load();

			// client.Start();
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
				// TODO: client.Disconnect("SIGTERM")
			}
		}

		#endregion
	}
}
