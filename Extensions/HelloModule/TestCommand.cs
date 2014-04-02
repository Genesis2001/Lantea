// -----------------------------------------------------------------------------
//  <copyright file="TestCommand.cs" company="Zack Loveless">
//      Copyright (c) Zack Loveless.  All rights reserved.
//  </copyright>
// -----------------------------------------------------------------------------

namespace HelloModule
{
	using System;
	using System.Linq;
	using Atlantis.Net.Irc;
	using Lantea.Common.Extensibility;

	public class TestCommand : ICommand
	{
		#region Implementation of ICommand

		public char Access
		{
			get { return (char)0; }
		}

		public string[] Triggers
		{
			get { return new[] {"!test", "!foo", "!hello"}; }
		}

		public bool CanExecute(int parameters)
		{
			return true;
		}

		public void Execute(IrcClient client, string nick, string target, params string[] args)
		{
			client.Message(target,
				"Hello, {0}. I see you've called my test command. Here's the list of parameters you sent with it: {1}",
				nick,
				String.Join(" ", args));
		}

		#endregion
	}
}
