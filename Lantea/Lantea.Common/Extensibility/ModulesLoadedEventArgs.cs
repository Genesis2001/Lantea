// -----------------------------------------------------------------------------
//  <copyright file="ModulesLoadedEventArgs.cs" company="Zack Loveless">
//      Copyright (c) Zack Loveless.  All rights reserved.
//  </copyright>
// -----------------------------------------------------------------------------

namespace Lantea.Common.Extensibility
{
	using System;
	using System.Collections.Generic;

	public class ModulesLoadedEventArgs : EventArgs
	{
		/// <summary>
		///     Initializes a new instance of the <see cref="T:ModulesLoadedEventArgs" /> class.
		/// </summary>
		public ModulesLoadedEventArgs(IEnumerable<IModule> modules)
		{
			Modules = modules;
		}

		public IEnumerable<IModule> Modules { get; private set; }
	}
}
