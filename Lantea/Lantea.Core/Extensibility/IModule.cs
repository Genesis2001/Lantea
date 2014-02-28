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
		IBotCore Bot { get; set; }
		
		void Initialize();

		void Rehash(Configuration config);
	}
}
