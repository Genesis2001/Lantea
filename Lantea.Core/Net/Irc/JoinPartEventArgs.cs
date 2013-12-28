// -----------------------------------------------------------------------------
//  <copyright file="JoinPartEventArgs.cs" company="Zack Loveless">
//      Copyright (c) Zack Loveless.  All rights reserved.
//  </copyright>
// -----------------------------------------------------------------------------
namespace Lantea.Core.Net.Irc
{
	using System;

	public class JoinPartEventArgs : EventArgs
	{
		public JoinPartEventArgs(string nick, string channel)
		{
			Channel = channel;
			Nick = nick;
		}

		public string Nick { get; private set; }

		public string Channel { get; private set; }
	}
}
