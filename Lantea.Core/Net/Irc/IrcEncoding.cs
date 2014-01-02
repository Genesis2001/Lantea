// -----------------------------------------------------------------------------
//  <copyright file="IrcEncoding.cs" company="Zack Loveless">
//      Copyright (c) Zack Loveless.  All rights reserved.
//  </copyright>
// -----------------------------------------------------------------------------

namespace Lantea.Core.Net.Irc
{
	public enum IrcEncoding
	{
		ASCII,
		UTF8,
	}

	public enum IrcHeaders
	{
		// Adding headers as-needed. Will eventually bulk-add them though.

		RPL_WELCOME       = 001,
		ERR_NICKNAMEINUSE = 433,
	}
}
