// -----------------------------------------------------------------------------
//  <copyright file="ICommandManager.cs" company="Zack Loveless">
//      Copyright (c) Zack Loveless.  All rights reserved.
//  </copyright>
// -----------------------------------------------------------------------------

namespace Lantea.Common.Extensibility
{
	using System.Collections.Generic;

	public interface ICommandManager
	{
		IList<ICommand> Commands { get; }

		void Initialize();

		void Register(ICommand command);
	}
}
