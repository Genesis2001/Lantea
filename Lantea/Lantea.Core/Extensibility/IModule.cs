// -----------------------------------------------------------------------------
//  <copyright file="IModule.cs" company="Zack Loveless">
//      Copyright (c) Zack Loveless.  All rights reserved.
//  </copyright>
// -----------------------------------------------------------------------------

namespace Lantea.Core.Extensibility
{
	using System;

	public interface IModule : IModuleAttribute
	{
		IBotCore Bot { get; set; }

		/// <summary>
		///	Initializes the module for consumption.
		/// </summary>
		void Initialize();
	}
}
