// -----------------------------------------------------------------------------
//  <copyright file="IModule.cs" company="Zack Loveless">
//      Copyright (c) Zack Loveless.  All rights reserved.
//  </copyright>
// -----------------------------------------------------------------------------

namespace Lantea.Core.Modules
{
	public interface IModule : IModuleMeta
	{
		Bot Bot { get; }

		void Load();

		void Unload();
	}
}
