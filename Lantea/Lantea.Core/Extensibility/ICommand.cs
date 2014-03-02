// -----------------------------------------------------------------------------
//  <copyright file="ICommand.cs" company="Zack Loveless">
//      Copyright (c) Zack Loveless.  All rights reserved.
//  </copyright>
// -----------------------------------------------------------------------------

namespace Lantea.Core.Extensibility
{
	using System.ComponentModel.Composition;

	[InheritedExport(typeof (ICommand))]
	public interface ICommand
	{
		// Modeled after System.Windows.ICommand.

		string[] Triggers { get; }

		string Description { get; }

		bool CanExecute(object parameter);

		void Execute(object parameter);
	}
}
