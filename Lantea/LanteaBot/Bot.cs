// -----------------------------------------------------------------------------
//  <copyright file="Bot.cs" company="Zack Loveless">
//      Copyright (c) Zack Loveless.  All rights reserved.
//  </copyright>
// -----------------------------------------------------------------------------

namespace Lantea
{
	using System;
	using System.Collections.Generic;
	using System.ComponentModel.Composition.Hosting;
	using System.IO;
	using System.Reflection;
	using Atlantis.Net.Irc;
	using Core.Extensibility;
	using Core.IO;

	// TODO: Migrate this class to it's own subassembly that can be swapped out for a user's own custom IBotCore.
	// TODO: Migrate this class to "Lantea.Core." Migrate "Lantea.Core" to "Lantea.Common" instead.
	public class Bot : IBotCore, IModuleLoader
	{
		#region Implementation of IDisposable

		/// <summary>
		/// Performs application-defined tasks associated with freeing, releasing, or resetting unmanaged resources.
		/// </summary>
		public void Dispose()
		{
			Dispose(true);
			GC.SuppressFinalize(this);
		}

		protected virtual void Dispose(Boolean disposing)
		{
			if (!disposing) return;

			if (Client != null) Client.Dispose();
		}

		#endregion

		#region Implementation of IBotCore

		public IrcClient Client { get; private set; }

		public Configuration Config { get; private set; }

		public IModuleLoader ModuleLoader
		{
			get { return this; }
		}

		public void Initialize(String configFile)
		{
			Config = new Configuration();
			Config.Load(configFile);

			Block connection = Config.GetBlock("connection");
			if (connection == null)
			{
				throw new Exception("No connection block found.");
			}

			String clientNick = connection.Get("nick", "Lantea");

			Client = new IrcClient(clientNick)
			         {
				         Host = connection.Get("host", "127.0.0.1"),
				         Port = connection.Get("port", 6667),
				         RealName = connection.Get("name", clientNick),
			         };

			var loc = Assembly.GetEntryAssembly().Location;
			var path = Path.GetDirectoryName(loc);
			// ReSharper disable once AssignNullToNotNullAttribute
			String moduleDirectory = Path.Combine(path, "Extensions");

			Block modules = Config.GetBlock("modules");
			if (modules != null)
			{
				moduleDirectory = modules.Get("directory", moduleDirectory);
			}

			ModuleLoader.LoadModules(moduleDirectory);

			Client.Start();
		}

		public void Load(String moduleName)
		{
		}

		#endregion

		#region Implementation of IModuleLoader

		public IEnumerable<IModule> Modules { get; private set; }

		public void LoadModules(String directory)
		{
			if (File.Exists(directory))
			{
				throw new DirectoryNotFoundException(String.Format("Unable to load modules from the specified path: {0}\nA file was given.", directory));
			}

			if (!Directory.Exists(directory))
			{
				throw new DirectoryNotFoundException(String.Format("Unable to load modules from the specified path: {0}\nThe directory does not exist.", directory));
			}

			var catalog = new DirectoryCatalog(directory);
		}

		#endregion
	}
}
