// -----------------------------------------------------------------------------
//  <copyright file="IModule.cs" company="Zack Loveless">
//      Copyright (c) Zack Loveless.  All rights reserved.
//  </copyright>
// -----------------------------------------------------------------------------

namespace Lantea.Common.Extensibility
{
	using IO;

	public interface IModule : IModuleAttribute
	{
		IBotCore Bot { get; }

		Configuration Config { get; }

		void Load();
	}
}
