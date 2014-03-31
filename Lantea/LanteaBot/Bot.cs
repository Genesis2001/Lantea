// -----------------------------------------------------------------------------
//  <copyright file="Bot.cs" company="Zack Loveless">
//      Copyright (c) Zack Loveless.  All rights reserved.
//  </copyright>
// -----------------------------------------------------------------------------

namespace LanteaBot
{
	using System;
	using System.Collections.Generic;
	using System.ComponentModel.Composition;
	using System.ComponentModel.Composition.Hosting;
	using System.IO;
	using System.Linq;
	using System.Reflection;
	using Atlantis.Net.Irc;
	using Lantea.Core.Extensibility;
	using Lantea.Core.IO;

	public class Bot : IBotCore
	{
		[ImportMany] private IEnumerable<Lazy<IModule, IModuleAttribute>> modules;
		
		private void Compose()
		{
			var container = GetCompositionContainer();
			container.ComposeExportedValue<IBotCore>(this);

			modules = container.GetExports<IModule, IModuleAttribute>();
		}

		private static CompositionContainer GetCompositionContainer()
		{
			// ReSharper disable AssignNullToNotNullAttribute
			Assembly asm     = Assembly.GetAssembly(typeof(Bot));
			string bLocation = Path.GetDirectoryName(asm.Location);
			string mLocation = Path.Combine(bLocation, "Extensions");
			// ReSharper restore AssignNullToNotNullAttribute

			if (!Directory.Exists(mLocation))
			{
				Directory.CreateDirectory(mLocation);
			}

			var catalog = new AggregateCatalog(new DirectoryCatalog(mLocation));

			return new CompositionContainer(catalog);
		}

		// ReSharper disable once InconsistentNaming
		private void LoadIRC()
		{
			Block connection = Config.GetBlock("connection");

			if (connection == null)
			{
				throw new Exception("No connection block found in config.");
			}

			String nick = connection.Get<String>("nick");

			Client = new IrcClient(nick)
			         {
				         Host     = connection.Get<String>("server"),
				         Port     = connection.Get<Int32>("port"),
				         RealName = connection.Get<String>("name"),
			         };
		}

		#region Implementation of IBotCore

		public IrcClient Client { get; private set; }
		
		public Configuration Config { get; private set; }

		public IEnumerable<IModule> Modules
		{
			get { return modules != null ? modules.Select(x => x.Value) : Enumerable.Empty<IModule>(); }
		}
		
		public void Initialize()
		{
			if (Config == null)
			{
				throw new InvalidOperationException("Unable to initialize core. Configuration not loaded.");
			}

			Compose();
			LoadIRC();

			if (Client != null)
			{
				Client.ConnectionEstablishedEvent += OnClientConnect;
				Client.Start();

				foreach (IModule m in Modules)
				{
					// 
				}
			}
		}

	    private void OnClientConnect(object sender, EventArgs args)
	    {
            Console.WriteLine("Connection established to IRC server.");
		}

		public void Load(string path, string module = null)
		{
			Config = new Configuration();

			Config.ConfigurationLoadEvent += OnRehash;
			Config.Load(path);
		}

		private void OnRehash(Object sender, ConfigurationLoadEventArgs args)
		{
			if (args.Success)
			{
				foreach (IModule m in Modules)
				{
					// 
				}
			}
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

			Client.Disconnect("Exiting.");
		}

		#endregion
	}
}
