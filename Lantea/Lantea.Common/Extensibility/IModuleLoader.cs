// -----------------------------------------------------------------------------
//  <copyright file="IModuleLoader.cs" company="Zack Loveless">
//      Copyright (c) Zack Loveless.  All rights reserved.
//  </copyright>
// -----------------------------------------------------------------------------

namespace Lantea.Common.Extensibility
{
	using System;
	using System.Collections.Generic;

	public interface IModuleLoader
	{
		event EventHandler<ModulesLoadedEventArgs> ModulesLoadedEvent;

		IEnumerable<IModule> Modules { get; }

		void LoadModules(String directory);
	}
}
