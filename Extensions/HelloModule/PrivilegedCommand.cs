// -----------------------------------------------------------------------------
//  <copyright file="PrivilegedCommand.cs" company="Zack Loveless">
//      Copyright (c) Zack Loveless.  All rights reserved.
//  </copyright>
// -----------------------------------------------------------------------------

namespace Hello
{
	using Atlantis.Net.Irc;
	using Lantea.Common.Extensibility;

	public class PrivilegedCommand : ICommand
	{
		#region Implementation of ICommand

		public char Access
		{
			get { return '&'; }
		}

		public string[] Triggers
		{
			get { return new[] {"!privcmd"}; }
		}

		public bool CanExecute(int parameters)
		{
			return true;
		}

		public void Execute(IrcClient client, string nick, string target, params string[] args)
		{
			client.Message(target, "Congrats, {0}. You've access a privileged command.", nick);
		}

		#endregion
	}
}
