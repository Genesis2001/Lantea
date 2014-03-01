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
	using System.Text;
	using System.Threading.Tasks;
	using Atlantis.Linq;
	using Atlantis.Net.Irc;
	using Lantea.Core.Extensibility;
	using Lantea.Core.IO;

	public class Bot : IBotCore
	{
	    private DateTime start;
	    private DateTime lastDisconnected;
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
			Assembly asm = Assembly.GetAssembly(typeof(Bot));
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
				Client.MessageReceivedEvent += OnMessageReceived;
			    Client.TimeoutEvent += OnTimeoutDetected;
				Client.Start();

				foreach (IModule m in Modules)
				{
					m.Initialize();
				}
			}
		}

	    private void OnTimeoutDetected(object sender, TimeoutEventArgs e)
	    {
	        Console.WriteLine("Timeout detected.");
	        lastDisconnected = DateTime.Now;
	    }

	    private void OnClientConnect(object sender, EventArgs args)
	    {
	        start = lastDisconnected = DateTime.Now;
            Console.WriteLine("Connection established to IRC server.");

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

		private void OnMessageReceived(object sender, MessageReceivedEventArgs args)
		{
            if (args.Message.StartsWith("!perm") && (!args.Target.EqualsIgnoreCase("#renegadex")))
			{
				Channel c = Client.GetChannel(args.Target);
				PrefixList perms;
				if (c.Users.TryGetValue(args.Source, out perms))
				{
					Client.Message(c.Name,
						"{0}, I see you have '{1}' as your highest prefix. You also have '{2}' as your prefix(es).",
						args.Source,
						perms.HighestPrefix,
						perms.ToString());
				}
			}
            else if (args.Message.StartsWith("!dump") && (args.Target.EqualsIgnoreCase("#Genesis") || args.Target.EqualsIgnoreCase("#UnifiedTech")))
            {
                string[] toks = args.Message.Split(' ');

                StringBuilder builder = new StringBuilder();
                foreach (Channel c in Client.Channels)
                {
                    if (toks.Length >= 2)
                    {
                        if (toks[1].StartsWith("#") && !c.Name.EqualsIgnoreCase(toks[1])) continue;
                    }

                    builder.AppendFormat("PRIVMSG {0} :{1}: ", args.Target, c.Name);

                    foreach (KeyValuePair<string, PrefixList> p in c.Users)
                    {
                        if (builder.Length >= 130)
                        {
                            string buffer = builder.ToString().TrimEnd(' ', ',');
                            Client.Send(buffer);

                            builder.Clear();
                            builder.AppendFormat("PRIVMSG {0} :{1}: ", args.Target, c.Name);
                        }

                        builder.Append(p.Value.HighestPrefix);
                        builder.Append(p.Key);
                        builder.Append(", ");
                    }

                    builder.Append('\n');
                }
            }
            else if (args.Message.StartsWith("!uptime") && (args.Target.EqualsIgnoreCase("#Genesis") || args.Target.EqualsIgnoreCase("#UnifiedTech")))
            {
                TimeSpan connected = (DateTime.Now - lastDisconnected);
                StringBuilder builder = new StringBuilder();

                if (connected == TimeSpan.Zero) builder.Append("0 minutes");

                if (connected.Days > 0) builder.AppendFormat("{0} days, ", connected.Days);
                if (connected.Hours > 0) builder.AppendFormat("{0} hours, ", connected.Hours);
                if (connected.Minutes > 0) builder.AppendFormat("{0} minutes, ", connected.Minutes);
                if (connected.Seconds > 0) builder.AppendFormat("{0} seconds, ", connected.Seconds);

                string buffer = builder.ToString().TrimEnd(',', ' ');

                Client.Message(args.Target, buffer);
            }
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
