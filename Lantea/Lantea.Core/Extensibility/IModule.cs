// -----------------------------------------------------------------------------
//  <copyright file="IModule.cs" company="Zack Loveless">
//      Copyright (c) Zack Loveless.  All rights reserved.
//  </copyright>
// -----------------------------------------------------------------------------

namespace Lantea.Core.Extensibility
{
	using System.ComponentModel.Composition;
	using IO;

	[InheritedExport(typeof (IModule))]
	public interface IModule
	{
		IBotCore Bot { get; set; }

		void Initialize();

		void Rehash(Configuration config);
	}
}
