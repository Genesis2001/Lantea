// -----------------------------------------------------------------------------
//  <copyright file="IModuleMeta.cs" company="Zack Loveless">
//      Copyright (c) Zack Loveless.  All rights reserved.
//      
//      LICENSE TBA
//  </copyright>
// -----------------------------------------------------------------------------

namespace Lantea.Common.Modules
{
	using System;

	public interface IModuleMeta
	{
		string Author { get; }

		string Version { get; }

		bool IsEnabled { get; set; }
	}
}
