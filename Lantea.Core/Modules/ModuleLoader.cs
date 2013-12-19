// -----------------------------------------------------------------------------
//  <copyright file="ModuleLoader.cs" company="Zack Loveless">
//      Copyright (c) Zack Loveless.  All rights reserved.
//      
//      LICENSE TBA
//  </copyright>
// -----------------------------------------------------------------------------

namespace Lantea.Core.Modules
{
	using System.Collections.Generic;
	using System.Collections.ObjectModel;
	using System.Collections.Specialized;
	using System.Linq;
	using Common.Modules;

	public interface IModuleLoader
	{
		IEnumerable<IModule> Modules { get; }

		void Compose();
	}

	public class ModuleLoader
	{
		private readonly ObservableCollection<IModule> modules;

		public ModuleLoader()
		{
			modules = new ObservableCollection<IModule>();
			modules.CollectionChanged += OnModuleCollectionChanged;
		}

		private void OnModuleCollectionChanged(object sender, NotifyCollectionChangedEventArgs args)
		{
			switch (args.Action)
			{
				case NotifyCollectionChangedAction.Add:
				{
					var addedModules = args.NewItems.OfType<IModule>();
					foreach (var item in addedModules) item.Load();
				} break;

				case NotifyCollectionChangedAction.Remove:
				{
					var removedModules = args.OldItems.OfType<IModule>();
					foreach (var item in removedModules) item.Unload();
				} break;
			}
		}
	}
}
