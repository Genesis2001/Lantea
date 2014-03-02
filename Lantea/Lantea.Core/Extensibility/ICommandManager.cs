// -----------------------------------------------------------------------------
//  <copyright file="ICommandManager.cs" company="Zack Loveless">
//      Copyright (c) Zack Loveless.  All rights reserved.
//  </copyright>
// -----------------------------------------------------------------------------

namespace Lantea.Core.Extensibility
{
	using System.Collections.Generic;

	public interface ICommandManager
	{
		IModule Owner { get; }

		IList<ICommand> Commands { get; }

		void LoadCommands();
	}
}
