namespace Lantea
{
	using System;
	using System.Collections.Generic;
	using System.ComponentModel.Composition.Hosting;
	using System.IO;
	using Common;
	using Common.Extensibility;

	public class ModuleLoader : IModuleLoader
	{
		public const String DefaultLocation = "Extensions";

		private Boolean disposed = false;
		private CompositionContainer container;

		private void CheckContainer()
		{
			if (disposed)
			{
				throw new ObjectDisposedException("container");
			}

			if (container == null)
			{
				Initialize();
			}
		}

		#region Implementation of IModuleLoader

		public event EventHandler<ModulesLoadedEventArgs> ModulesLoadedEvent;

		/// <summary>
		/// Gets or sets a <see cref="T:System.String" /> value path relative to the main entry point.
		/// </summary>
		public string Location { get; set; }

		/// <summary>
		/// Gets a collection of modules loaded by the module loader.
		/// </summary>
		public IEnumerable<IModule> Modules { get; private set; }

		/// <summary>
		/// Returns a collection of T from the composition container.
		/// </summary>
		/// <typeparam name="T"></typeparam>
		/// <returns></returns>
		public IEnumerable<T> Get<T>()
		{
			CheckContainer();

			throw new NotImplementedException();
		}

		/// <summary>
		/// Returns a collection of T from the composition container that have the specified metadata
		/// </summary>
		/// <typeparam name="T"></typeparam>
		/// <typeparam name="TMetadata"></typeparam>
		/// <param name="condition"></param>
		/// <returns></returns>
		public IEnumerable<T> Get<T, TMetadata>(Func<TMetadata, bool> condition = null)
		{
			CheckContainer();

			throw new NotImplementedException();
		}

		/// <summary>
		/// Returns a single contract from the composition container.
		/// </summary>
		/// <typeparam name="T"></typeparam>
		/// <returns></returns>
		public T GetSingle<T>()
		{
			CheckContainer();

			throw new NotImplementedException();
		}

		/// <summary>
		/// Initializes the composition container and loads the bot.
		/// </summary>
		public void Initialize()
		{
			if (File.Exists(Location))
			{
				throw new PathNotFoundException("The specified path was not found. A file was given.", Location);
				//throw new DirectoryNotFoundException(String.Format("Unable to load modules from the specified path: {0}\nA file was given.", Location));
			}

			if (!Directory.Exists(Location))
			{
				throw new PathNotFoundException("The specified path was not found. The directory does not exist.", Location);
			}

			var catalog = new AggregateCatalog(new DirectoryCatalog("."), new DirectoryCatalog(Location));
			container = new CompositionContainer(catalog);
		}

		/// <summary>
		/// Loads discovered modules.
		/// </summary>
		public void Load()
		{
			CheckContainer();

			throw new NotImplementedException();
		}

		#endregion

		#region Implementation of IIoCContainer

		private readonly Dictionary<Type, Object> registeredTypes = new Dictionary<Type, Object>(); 

		void IIoCContainer.RegisterContract<T>(T contract)
		{
			if (registeredTypes.ContainsKey(typeof (T)))
			{
				throw new InvalidOperationException("Unable to register the specified contract. One already exists.");
			}

			registeredTypes.Add(typeof (T), contract);
		}

		T IIoCContainer.RetrieveContract<T>()
		{
			object contract;
			return registeredTypes.TryGetValue(typeof (T), out contract) ? (T)contract : default(T);
		}

		#endregion
	}

	public class PathNotFoundException : Exception
	{
		public PathNotFoundException(String message, String path) : base(String.Format("{0}\n\nPath: {1}", message, path))
		{
		}
	}
}
