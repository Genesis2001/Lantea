// -----------------------------------------------------------------------------
//  <copyright file="ModuleBase.cs" company="Zack Loveless">
//      Copyright (c) Zack Loveless.  All rights reserved.
//  </copyright>
// -----------------------------------------------------------------------------

namespace Lantea.Core.Extensibility
{
	using IO;

	public abstract class ModuleBase : IModule
	{
		protected ModuleBase(IBotCore bot)
		{
			Bot = bot;
		}

		#region Implementation of IModule

		public IBotCore Bot { get; set; }

		public abstract void Initialize();

		public abstract void Rehash(Configuration config);

		#endregion

		#region Implementation of IModuleAttribute

		public abstract string Name { get; }

		public abstract string Author { get; }

		public abstract ModuleType Type { get; }

		public abstract string Version { get; }

		#endregion
	}
}
