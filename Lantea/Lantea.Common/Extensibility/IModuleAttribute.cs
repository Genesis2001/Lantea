// -----------------------------------------------------------------------------
//  <copyright file="IModuleAttribute.cs" company="Zack Loveless">
//      Copyright (c) Zack Loveless.  All rights reserved.
//  </copyright>
// -----------------------------------------------------------------------------

namespace Lantea.Common.Extensibility
{
	using System;

	public interface IModuleAttribute
	{
		String Author { get; }

		String Description { get; }

		String Name { get; }

		String Version { get; }

		ModuleType Type { get; }
	}
}
