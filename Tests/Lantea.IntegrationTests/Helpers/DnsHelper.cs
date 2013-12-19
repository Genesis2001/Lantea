// -----------------------------------------------------------------------------
//  <copyright file="DnsHelper.cs" company="Zack Loveless">
//      Copyright (c) Zack Loveless.  All rights reserved.
//      
//      LICENSE TBA
//  </copyright>
// -----------------------------------------------------------------------------
namespace Lantea.IntegrationTests.Helpers
{
	using System.Net;

	public static class DnsHelper
	{
		// public static Func<String, IPHostEntry> GetHostEntry;

		public static IPHostEntry GetHostEntry(string hostNameOrAddress)
		{
			if (hostNameOrAddress.Equals("irc.unifiedtech.org"))
			{
				return new IPHostEntry
				       {
						   AddressList = new IPAddress[] { null, null },
				       };
			}

			return null;
		}
	}
}
