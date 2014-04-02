// -----------------------------------------------------------------------------
//  <copyright file="IrcClientExtensions.cs" company="Zack Loveless">
//      Copyright (c) Zack Loveless.  All rights reserved.
//  </copyright>
// -----------------------------------------------------------------------------

namespace Atlantis.Net.Irc
{
	using System;

	public partial class IrcClient
	{
		public bool IsHigherOrEqualToPrefix(char prefixA, char prefixB)
		{
			if (prefixA == prefixB) return true;

			char[] accessPrefixes = AccessPrefixes.ToCharArray();
			Array.Reverse(accessPrefixes);

			String prefixes = new string(accessPrefixes);
			return prefixes.IndexOf(prefixA) >= prefixes.IndexOf(prefixB);
		}
	}
}
