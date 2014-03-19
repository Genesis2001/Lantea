// -----------------------------------------------------------------------------
//  <copyright file="IModule.cs" company="Zack Loveless">
//      Copyright (c) Zack Loveless.  All rights reserved.
//  </copyright>
// -----------------------------------------------------------------------------

namespace Lantea.Core.Extensibility
{
	using System.ComponentModel.Composition;
	using IO;

	[InheritedExport("Module", typeof(IModule))]
	public interface IModule : IModuleAttribute
	{
		IBotCore Bot { get; }

		void Initialize();

		void Rehash(Configuration config);
	}
}
