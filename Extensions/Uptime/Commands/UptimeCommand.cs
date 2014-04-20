// -----------------------------------------------------------------------------
//  <copyright file="UptimeCommand.cs" company="Zack Loveless">
//      Copyright (c) Zack Loveless.  All rights reserved.
//  </copyright>
// -----------------------------------------------------------------------------

namespace Uptime.Commands
{
	using System;
	using System.Linq;
	using System.Text.RegularExpressions;
	using Atlantis.Net.Irc;
	using Lantea.Common.Extensibility;
	using Services;

	public class UptimeCommand : ICommand
	{
		#region Implementation of ICommand

		public char Access
		{
			get { return (char)0; }
		}

		public string[] Triggers
		{
			get { return new[] {"!uptime"}; }
		}

		public bool CanExecute(int parameters)
		{
			return parameters >= 1;
		}

		public void Execute(IrcClient client, string nick, string target, params string[] args)
		{
			Service[] services = args[0] == "*"
				? ServiceManager.Instance.ToArray() // "*" is representative of *all* the services. No need to use a regex to find *all* when we have them all.
				: ServiceManager.Instance.Where(x => Regex.IsMatch(x.DisplayName, args[0])).ToArray();

			if (services.Length == 0)
			{
				client.Message(target, "No services match the specified query.");
			}
			else
			{
				foreach (var item in services)
				{
					client.Message(target, "{0}: {1}", item.DisplayName.Bold(), item.Check());
				}
			}
		}

		#endregion
	}
}
