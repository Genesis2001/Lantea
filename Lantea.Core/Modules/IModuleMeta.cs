// -----------------------------------------------------------------------------
//  <copyright file="IModuleMeta.cs" company="Zack Loveless">
//      Copyright (c) Zack Loveless.  All rights reserved.
//  </copyright>
// -----------------------------------------------------------------------------

namespace Lantea.Core.Modules
{
	public interface IModuleMeta
	{
		string Author { get; }

		bool IsEnabled { get; set; }
	}
}
