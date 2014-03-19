// -----------------------------------------------------------------------------
//  <copyright file="ModuleBase.cs" company="Zack Loveless">
//      Copyright (c) Zack Loveless.  All rights reserved.
//  </copyright>
// -----------------------------------------------------------------------------

namespace Lantea.Core.Extensibility
{
	using System;
	using System.Collections.Generic;
	using System.Collections.ObjectModel;
	using System.ComponentModel.Composition;
	using System.ComponentModel.Composition.Hosting;
	using IO;

	public abstract class ModuleBase : IModule, ICommandManager
	{
		protected Configuration config;

		protected ModuleBase(IBotCore bot)
		{
			Bot      = bot;
			Commands = new ObservableCollection<ICommand>();
		}

		#region Implementation of ICommandManager

		public IModule Owner
		{
			get { return this; }
		}

		public IList<ICommand> Commands { get; private set; }

		public virtual void LoadCommands()
		{
			var asm = GetType().Assembly;
			var catalog = new AssemblyCatalog(asm);
			var container = new CompositionContainer(catalog);
			container.ComposeExportedValue<IModule>(this);

			var commands = container.GetExportedValues<ICommand>();

			foreach (ICommand c in commands)
			{
				Commands.Add(c);
			}
		}

		#endregion

		#region Implementation of IModule

		public IBotCore Bot { get; set; }

		public abstract void Initialize();

		public virtual void Rehash(Configuration config)
		{
			if (config == null) throw new ArgumentNullException("config");

			this.config = config;
		}

		#endregion

		#region Implementation of IModuleAttribute

		public abstract string Name { get; }

		public abstract string Author { get; }

		public abstract ModuleType Type { get; }

		public abstract string Version { get; }

		#endregion
	}
}
