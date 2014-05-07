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
	using Common;
	using Common.Extensibility;

	public class BootstrapEx : IModuleLoader, IDisposable
	{
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
					System.Threading.Thread.Sleep(100);

					ConsoleKeyInfo key = Console.ReadKey(true);
					if (key.Modifiers == ConsoleModifiers.Control && key.Key == ConsoleKey.C)
					{
						exit = true;
					}
				} while (!exit);
			}

#if DEBUG
			Console.Write("Bot running in debug mode. Press <ENTER> to exit completely.");
			Console.ReadLine();
#endif
		}

		public void Run(String[] args)
		{
			
		}

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

		private Boolean disposed = false;
		private CompositionContainer container;

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
			throw new NotImplementedException();
		}

		/// <summary>
		/// Returns a single item from the composition container.
		/// </summary>
		/// <typeparam name="T"></typeparam>
		/// <returns></returns>
		public T GetSingle<T>()
		{
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
			throw new NotImplementedException();
		}

		#endregion

		#region Implementation of IDisposable

		/// <summary>
		/// Performs application-defined tasks associated with freeing, releasing, or resetting unmanaged resources.
		/// </summary>
		public void Dispose()
		{
		}

		#endregion
	}
}
