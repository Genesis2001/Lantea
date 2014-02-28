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
	using System.Configuration;
	using System.IO;
	using System.Linq;
	using System.Reflection;
	using System.Threading.Tasks;
	using Atlantis.Net.Irc;
	using Lantea.Core.Extensibility;
	using Lantea.Core.IO;

	public class Bot : IBotCore
	{
		[ImportMany] private IEnumerable<Lazy<IModule, IModuleAttribute>> modules;
		
		private void Compose()
		{
			var container = GetCompositionContainer();

			modules = container.GetExports<IModule, IModuleAttribute>();
		}

		// ReSharper disable once InconsistentNaming
		private void LoadIRC()
		{
			Block uplink = Config.GetBlock("connection");

			if (uplink == null)
			{
				throw new Exception("No connection block found in config.");
			}

			string nick = uplink.Get<String>("nick");

			Client = new IrcClient(nick)
			         {
				         Host     = uplink.Get<String>("server"),
				         Port     = uplink.Get<Int32>("port"),
				         RealName = uplink.Get<String>("name"),
			         };
		}

		#region Implementation of IBotCore

		public IrcClient Client { get; private set; }
		
		public Configuration Config { get; private set; }

		public IEnumerable<IModule> Modules
		{
			get { return modules != null ? modules.Select(x => x.Value) : Enumerable.Empty<IModule>(); }
		}
		
		private static CompositionContainer GetCompositionContainer()
		{
			// ReSharper disable AssignNullToNotNullAttribute
			var asm          = Assembly.GetAssembly(typeof (Bot));
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
					m.Initialize();
				}
			}
		}

		private void OnClientConnect(object sender, EventArgs args)
		{
			Task.Factory.StartNew(() =>
			                      {
				                      if (!Client.EnableFakeLag)
				                      {
										  Task.Delay(15000).Wait();
				                      }

									  // TODO: read list of perform commands from config.
				                      Client.Send("JOIN #UnifiedTech");
			                      });
		}

		public Configuration Load(string path)
		{
			Config = new Configuration();

			Config.ConfigurationLoadEvent += OnRehash;
			Config.Load(path);

			return Config;
		}

		private void OnRehash(object sender, ConfigurationLoadEventArgs args)
		{
			if (args.Success)
			{
				foreach (IModule m in Modules)
				{
					m.Rehash(Config);
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
