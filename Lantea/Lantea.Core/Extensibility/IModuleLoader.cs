// -----------------------------------------------------------------------------
//  <copyright file="IModuleLoader.cs" company="Zack Loveless">
//      Copyright (c) Zack Loveless.  All rights reserved.
//  </copyright>
// -----------------------------------------------------------------------------

namespace Lantea.Core.Extensibility
{
	using System;
	using System.Collections.Generic;

	public interface IModuleLoader
	{
		IEnumerable<IModule> Modules { get; }

		void LoadModules(String directory);
	}
}
