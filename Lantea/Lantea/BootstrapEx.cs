// -----------------------------------------------------------------------------
//  <copyright file="BootstrapEx.cs" company="Zack Loveless">
//      Copyright (c) Zack Loveless.  All rights reserved.
//  </copyright>
// -----------------------------------------------------------------------------

namespace Lantea
{
	using System;
	using System.Collections.Generic;
	using System.ComponentModel.Composition.Hosting;
	using System.IO;
	using System.Linq;
	using System.Threading;
	using Atlantis.IO;
	using Common;
	using Common.Extensibility;
	using Common.IO;
	using NDesk.Options;

	public class BootstrapEx : IModuleLoader, IDisposable
	{
		#region Entry point

		public static void Main(string[] args)
		{
			Console.Title = "Lantea IRC Bot";
			Console.SetWindowSize(125, 30);

			using (BootstrapEx b = new BootstrapEx())
			{
				b.Run(args);

				Boolean exit = false;
				do
				{
					Thread.Sleep(100);

					ConsoleKeyInfo key = Console.ReadKey(true);
					if (key.Modifiers == ConsoleModifiers.Control && key.Key == ConsoleKey.C)
					{
						exit = true;
						b.Unload();
					}
				} while (!exit);
			}

#if DEBUG
			Console.Write("Bot running in debug mode. Press <ENTER> to exit completely.");
			Console.ReadLine();
#endif
		}

		private static void ShowUsage(OptionSet options, object param = null)
		{
			Console.WriteLine("Usage: Lantea.exe [OPTIONS] --config=<relative path>");

			if (param is OptionException)
			{
				var e = param as OptionException;

				Console.WriteLine();
				Console.WriteLine("The option argument '{0}' is required by Lantea.", e.OptionName);
			}

			Console.WriteLine();
			options.WriteOptionDescriptions(Console.Out);
		}

		#endregion

		public BootstrapEx()
		{
			Log = new ConsoleLog(Console.Write);

#if DEBUG
			Log.Threshold = LogThreshold.Verbose;
#else
			Log.Threshold = (LogThreshold)15; // Debug is 16. This sets all bits prior to debug, excluding debug.
#endif
		}

		public ILog Log { get; private set; }
		
		#region Methods

		private void CheckContainer()
		{
			if (container == null || disposed)
			{
				throw new ObjectDisposedException("container", "The composition container has been disposed.");
			}
		}

		public void Run(String[] args)
		{
			Boolean help = false;
			String configFile = null;
			String pluginsLocation = DefaultLocation;

			OptionSet options = new OptionSet
			                    {
				                    {"config=", "Sets the path for the configuration file to be used by Lantea.", x => configFile = x},
				                    {"p:|plugins-dir:", "Sets the plugin directory for Lantea's module system.",x => pluginsLocation = x},
				                    {"?|h|help", "Shows this help system.", (bool x) => help = x},
			                    };
			try
			{
				options.Parse(args);

				if (help)
				{
					ShowUsage(options);
					return;
				}
			}
			catch (OptionException e)
			{
				ShowUsage(options, e);
				return;
			}

			Location = Path.Combine(AppDomain.CurrentDomain.SetupInformation.ApplicationBase, pluginsLocation);

			if (!Directory.Exists(Location))
			{
				Directory.CreateDirectory(Location);
			}

			configFile = Path.Combine(AppDomain.CurrentDomain.SetupInformation.ApplicationBase, configFile);
			if (!File.Exists(configFile))
			{
				Log.ErrorFormat("");
			}

			// Initialize the loader.
			Initialize();
			
			// Load modules.
			Load();
		}

		#endregion
		
		#region Implementation of IIoCContainer

		private readonly Dictionary<Type, Object> registeredTypes = new Dictionary<Type, Object>();

		void IIoCContainer.RegisterContract<T>(T contract)
		{
			if (registeredTypes.ContainsKey(typeof(T)))
			{
				throw new InvalidOperationException("Unable to register the specified contract. One already exists.");
			}

			registeredTypes.Add(typeof(T), contract);
		}

		T IIoCContainer.RetrieveContract<T>()
		{
			object contract;
			return registeredTypes.TryGetValue(typeof(T), out contract) ? (T)contract : default(T);
		}

		#endregion

		#region Implementation of IModuleLoader

		public const String DefaultLocation = "Extensions";
		
		private AggregateCatalog catalog;
		private Boolean disposed = false;
		private CompositionContainer container;

		private readonly List<Lazy<IModule, IModuleAttribute>> modules = new List<Lazy<IModule, IModuleAttribute>>();

		public event EventHandler<ModulesLoadedEventArgs> ModulesLoadedEvent;

		/// <summary>
		/// Gets or sets a <see cref="T:System.String" /> value path relative to the main entry point.
		/// </summary>
		public string Location { get; set; }

		/// <summary>
		/// Gets a collection of modules loaded by the module loader.
		/// </summary>
		public IEnumerable<IModule> Modules
		{
			get { return modules.Select(x => x.Value).ToArray(); }
		}

		/// <summary>
		/// Returns a collection of T from the composition container.
		/// </summary>
		/// <typeparam name="T"></typeparam>
		/// <returns></returns>
		public IEnumerable<T> GetExportedValues<T>()
		{
			CheckContainer();

			return container.GetExportedValues<T>();
		}

		/// <summary>
		/// Returns a collection of T from the composition container that have the specified metadata
		/// </summary>
		/// <typeparam name="T"></typeparam>
		/// <typeparam name="TMetadata"></typeparam>
		/// <returns></returns>
		public IEnumerable<Lazy<T, TMetadata>> GetExportedValues<T, TMetadata>()
		{
			CheckContainer();

			return container.GetExports<T, TMetadata>();
		}

		/// <summary>
		/// Returns a single item from the composition container.
		/// </summary>
		/// <typeparam name="T"></typeparam>
		/// <returns></returns>
		public T GetExportedValue<T>()
		{
			CheckContainer();

			return container.GetExportedValue<T>();
		}

		/// <summary>
		/// Initializes the composition container and loads the bot.
		/// </summary>
		public void Initialize()
		{
			if (File.Exists(Location))
			{
				throw new DirectoryNotFoundException(String.Format("The given path is a file, not a directory.\n\nPath: {0}", Location));
			}

			catalog   = new AggregateCatalog(new DirectoryCatalog(".", "*.dll"), new DirectoryCatalog(Location, "*.dll"));
			container = new CompositionContainer(catalog);
		}

		/// <summary>
		/// Loads discovered modules.
		/// </summary>
		public void Load()
		{
			if (catalog != null)
			{
				catalog.Catalogs.OfType<DirectoryCatalog>().ToList().ForEach(x => x.Refresh());
			}


		}

		/// <summary>
		/// Unloads modules.
		/// </summary>
		public void Unload()
		{
			throw new NotImplementedException();
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

		protected virtual void Dispose(Boolean disposing)
		{
			if (!disposing || disposed) return;

			Unload();

			if (container != null)
			{
				container.Dispose();
			}
		}

		#endregion
	}
}
