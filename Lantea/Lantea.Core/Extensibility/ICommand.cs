// -----------------------------------------------------------------------------
//  <copyright file="ICommand.cs" company="Zack Loveless">
//      Copyright (c) Zack Loveless.  All rights reserved.
//  </copyright>
// -----------------------------------------------------------------------------

namespace Lantea.Core.Extensibility
{
	using System.Collections.Generic;

	public interface ICommand
	{
		// Modeled after System.Windows.ICommand.

		IList<string> Triggers { get; }

		string Description { get; set; }

		bool CanExecute(object parameter);

		void Execute(object parameter);
	}
}
