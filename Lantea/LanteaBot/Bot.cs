﻿// -----------------------------------------------------------------------------
//  <copyright file="Bot.cs" company="Zack Loveless">
//      Copyright (c) Zack Loveless.  All rights reserved.
//  </copyright>
// -----------------------------------------------------------------------------

namespace LanteaBot
{
	using System;
	using System.Collections.Generic;
	using Atlantis.Net.Irc;
	using Lantea.Core.Extensibility;
	using Lantea.Core.IO;

	public class Bot : IBotCore
	{
		#region Implementation of IBotCore

		public IrcClient Client { get; private set; }
		
		public Configuration Config { get; private set; }
		
		public IEnumerable<Lazy<IModule, IModuleAttribute>> Modules { get; private set; }

		public void Initialize()
		{
			throw new NotImplementedException();
		}

		public Configuration Load(string path)
		{
			Config = new Configuration();
			Config.Load(path);

			return Config;
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

			// 
		}

		#endregion
	}
}
