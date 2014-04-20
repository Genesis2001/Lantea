// -----------------------------------------------------------------------------
//  <copyright file="Bot.cs" company="Zack Loveless">
//      Copyright (c) Zack Loveless.  All rights reserved.
//  </copyright>
// -----------------------------------------------------------------------------

namespace Lantea.Core
{
	using System;
	using System.Collections.Generic;
	using System.ComponentModel.Composition;
	using System.ComponentModel.Composition.Hosting;
	using System.IO;
	using System.Linq;
	using System.Reflection;
	using Atlantis.IO;
	using Atlantis.Linq;
	using Atlantis.Net.Irc;
	using Common.Extensibility;
	using Common.IO;
	using Common.Linq;

	// ReSharper disable InconsistentNaming
	public class Bot : IBotCore, IModuleLoader, ICommandManager
	{
		private void LoadIRC()
		{
			Log.Info("Loading Lantea IRC configuration.");
			Block connection = Config.GetBlock("connection");
			if (connection == null)
			{
				Log.Error("No connection block found in configuration.");
				return;
			}

			String clientNick = connection.Get("nick", "Lantea");

			Client = new IrcClient(clientNick)
			         {
				         Host     = connection.Get("server", "127.0.0.1"),
				         Port     = connection.Get("port", 6667),
				         RealName = connection.Get("name", clientNick),
			         };

			Log.InfoFormat("IRC configuration loaded: {0}:{1} as {2}", Client.Host, Client.Port, Client.Nick);
		}

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

			if (Client != null)
			{
				Client.Disconnect("Disconnecting.");
				Client.Dispose();
			}
		}

		#endregion

		#region Implementation of IBotCore

		public IrcClient Client { get; private set; }

		/// <summary>
		/// Gets an <see cref="T:ICommandManager" /> instance for the current bot.
		/// </summary>
		public ICommandManager CommandManager
		{
			get { return this; }
		}

		public Configuration Config { get; private set; }

		public ILog Log { get; private set; }

		public IModuleLoader ModuleLoader
		{
			get { return this; }
		}

		// ReSharper disable AssignNullToNotNullAttribute
		public void Initialize(String configFile)
		{
			Config = new Configuration();
			Config.Load(configFile);

			var loc     = Assembly.GetEntryAssembly().Location;
			String path = Path.GetDirectoryName(loc);

			String logPath = Path.Combine(path, "logs");
			if (!Directory.Exists(logPath))
			{
				Directory.CreateDirectory(logPath);
			}

			List<ILog> logs = new List<ILog>();
			for (Int32 i = 0; i <= Config.CountBlock("log"); ++i)
			{
				Block entry = Config.GetBlock("log", i);

				if (entry != null)
				{
					String target   = entry.GetString("target");
					Int32 threshold = entry.GetInt32("threshold");

					logs.Add(new FileLog(Path.Combine(logPath, target)) {Threshold = (LogThreshold)threshold});
				}
			}

			logs.Add(new ConsoleLog(Console.Write) {Threshold = LogThreshold.Verbose});
			Log = new MultiLog(logs) {PrefixLog = true};

			Log.Info("Starting Lantea bot.");
			LoadIRC();

			String moduleDirectory = Path.Combine(path, "Extensions");

			Block modules = Config.GetBlock("modules");
			if (modules != null)
			{
				moduleDirectory = modules.Get("directory", moduleDirectory);
			}

			moduleDirectory = Path.Combine(path, moduleDirectory);
			
			Client.Start();

			CommandManager.Initialize();

			ModuleLoader.ModulesLoadedEvent += OnModulesLoaded;
			ModuleLoader.LoadModules(moduleDirectory);
		}

		private void OnModulesLoaded(object sender, ModulesLoadedEventArgs args)
		{
			Log.Info("Modules loaded.");
		}

		public void Load(String moduleName)
		{
		}

		#endregion

		#region Implementation of IModuleLoader

		public event EventHandler<ModulesLoadedEventArgs> ModulesLoadedEvent;

		public IEnumerable<IModule> Modules { get; private set; }

		void IModuleLoader.LoadModules(String directory)
		{
			if (File.Exists(directory))
			{
				throw new DirectoryNotFoundException(String.Format("Unable to load modules from the specified path: {0}\nA file was given.", directory));
			}

			if (!Directory.Exists(directory))
			{
				throw new DirectoryNotFoundException(String.Format("Unable to load modules from the specified path: {0}\nThe directory does not exist.", directory));
			}

			var catalog   = new DirectoryCatalog(directory);
			var container = new CompositionContainer(catalog);

			container.ComposeExportedValue<IBotCore>(this);
			container.ComposeExportedValue(Config);

			var composedModules   = container.GetExports<IModule, IModuleAttribute>().ToList();
			List<IModule> modules = new List<IModule>();

			Log.Info("Loading modules.");
			foreach (var item in composedModules)
			{
				IModule value = item.Value;

				if (Config.GetModule(value) == null)
				{
					// module is commented out and/or not enabled in the config.
					continue;
				}

				value.Load();

				String description = String.IsNullOrEmpty(item.Metadata.Description)
					? item.Metadata.Name
					: item.Metadata.Description;

				Log.InfoFormat("Loaded: {0} (v{1}) by {2}", description, item.Metadata.Version, item.Metadata.Author);

				modules.Add(value);
			}

			Modules = modules;
			ModulesLoadedEvent.Raise(this, new ModulesLoadedEventArgs(Modules));
		}

		#endregion

		#region Implementation of ICommandManager

		public IList<ICommand> Commands { get; private set; }

		void ICommandManager.Initialize()
		{
			Commands = new List<ICommand>();

			Client.MessageReceivedEvent += OnChannelMessageReceived;
		}

		private void OnChannelMessageReceived(object sender, MessageReceivedEventArgs args)
		{
			if (!args.Target.StartsWith("#")) return;

			String[] toks = args.Message.Split(' ');
			if (toks.Length > 0)
			{
				Channel c = Client.GetChannel(args.Target);
				PrefixList list;
				if (!c.Users.TryGetValue(args.Source, out list))
				{
					return;
				}

				foreach (ICommand cmd in Commands.Where(cmd => cmd.Triggers.Any(x => x.Equals(toks[0], StringComparison.OrdinalIgnoreCase))).ToArray())
				{
					if (cmd.CanExecute(toks.Length - 1) && (Client.ComparePrefix(list.HighestPrefix, cmd.Access) >= 0 || cmd.Access == (char)0))
					{
						cmd.Execute(Client, args.Source, args.Target, toks.Skip(1).ToArray());
					}
					else if (Client.ComparePrefix(list.HighestPrefix, cmd.Access) < 0)
					{
						Client.Notice(args.Source, "Sorry, you don't have access to {0}", cmd.Triggers[0]);
					}

					/*if (cmd.CanExecute(toks.Length - 1) &&
						    Client.IsHigherOrEqualToPrefix(list.HighestPrefix, cmd.Access))
						{
							cmd.Execute(Client, args.Source, args.Target, toks.Skip(1).ToArray());
						}
						else if (!Client.IsHigherOrEqualToPrefix(list.HighestPrefix, cmd.Access))
						{
							Client.Notice(args.Source, "You need '{0}' to access this command. You currently have: {1}", cmd.Access, list);
						}*/
				}
			}
		}

		void ICommandManager.Register(ICommand command)
		{
			Commands.Add(command);
		}

		#endregion
	}
	// ReSharper restore InconsistentNaming
}
