// -----------------------------------------------------------------------------
//  <copyright file="ModuleType.cs" company="Zack Loveless">
//      Copyright (c) Zack Loveless.  All rights reserved.
//  </copyright>
// -----------------------------------------------------------------------------

namespace Lantea.Core.Extensibility
{
	public enum ModuleType : uint
	{
		THIRD  = 0, // Third-party module
		VENDOR = 1, // official vendor module
		EXTRA  = 2, // vendor created, addon module.
	}
}
