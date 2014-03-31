// -----------------------------------------------------------------------------
//  <copyright file="IModule.cs" company="Zack Loveless">
//      Copyright (c) Zack Loveless.  All rights reserved.
//  </copyright>
// -----------------------------------------------------------------------------

namespace Lantea.Core.Extensibility
{
	using IO;

	public interface IModule : IModuleAttribute
	{
		IBotCore Bot { get; }

		/// <summary>
		/// Gets a 
		/// </summary>
		Configuration Config { get; }

		/// <summary>
		/// Represents the entry-point for the module.
		/// </summary>
		void Load();
	}
}
