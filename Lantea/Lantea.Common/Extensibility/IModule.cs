// -----------------------------------------------------------------------------
//  <copyright file="IModule.cs" company="Zack Loveless">
//      Copyright (c) Zack Loveless.  All rights reserved.
//  </copyright>
// -----------------------------------------------------------------------------

namespace Lantea.Common.Extensibility
{
	using System.ComponentModel.Composition;
	using IO;

	[InheritedExport(typeof (IModule))]
	public interface IModule : IModuleAttribute
	{
		IBotCore Bot { get; }

		Configuration Config { get; }

		void Load();
	}
}
