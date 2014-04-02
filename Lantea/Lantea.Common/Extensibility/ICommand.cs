// -----------------------------------------------------------------------------
//  <copyright file="ICommand.cs" company="Zack Loveless">
//      Copyright (c) Zack Loveless.  All rights reserved.
//  </copyright>
// -----------------------------------------------------------------------------

namespace Lantea.Common.Extensibility
{
	using System;
	using System.ComponentModel.Composition;
	using Atlantis.Net.Irc;

	[InheritedExport(typeof (ICommand))]
	public interface ICommand
	{
		Char Access { get; }

		String[] Triggers { get; }

		Boolean CanExecute(Int32 parameters);

		void Execute(IrcClient client, String nick, String target, params String[] args);
	}
}
