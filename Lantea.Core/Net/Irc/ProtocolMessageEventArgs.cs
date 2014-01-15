// -----------------------------------------------------------------------------
//  <copyright file="ProtocolMessageEventArgs.cs" company="Zack Loveless">
//      Copyright (c) Zack Loveless.  All rights reserved.
//  </copyright>
// -----------------------------------------------------------------------------

namespace Lantea.Core.Net.Irc
{
	using System;
	using System.Text.RegularExpressions;

	public class ProtocolMessageEventArgs : EventArgs
	{
		public ProtocolMessageEventArgs(Match match, string message)
		{
			Message = message;
			Match   = match;
		}

		public Match Match { get; private set; }

		public string Message { get; private set; }
	}
}
