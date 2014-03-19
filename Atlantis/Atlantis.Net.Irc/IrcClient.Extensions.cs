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
		public bool Compare(char prefixA, char prefixB)
		{
			return accessPrefixes.IndexOf(prefixA) > accessPrefixes.IndexOf(prefixB);
		}
	}
}
