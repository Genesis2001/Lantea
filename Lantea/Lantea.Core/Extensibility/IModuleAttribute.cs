// -----------------------------------------------------------------------------
//  <copyright file="IModuleAttribute.cs" company="Zack Loveless">
//      Copyright (c) Zack Loveless.  All rights reserved.
//  </copyright>
// -----------------------------------------------------------------------------

namespace Lantea.Core.Extensibility
{
	public interface IModuleAttribute
	{
		string Name { get; }

		string Author { get; }

		ModuleType Type { get; }

		string Version { get; }
	}
}
