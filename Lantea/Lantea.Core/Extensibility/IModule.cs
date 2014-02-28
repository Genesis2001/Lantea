// -----------------------------------------------------------------------------
//  <copyright file="IModule.cs" company="Zack Loveless">
//      Copyright (c) Zack Loveless.  All rights reserved.
//  </copyright>
// -----------------------------------------------------------------------------

namespace Lantea.Core.Extensibility
{
	using System.Collections.Generic;
	using IO;

	public interface IModule : IModuleAttribute
	{
		IBotCore Bot { get; set; }

		IList<ICommand> Commands { get; } 
		
		void Initialize();

		void Rehash(Configuration config);
	}
}
