// -----------------------------------------------------------------------------
//  <copyright file="IrcClientHelper.cs" company="Zack Loveless">
//      Copyright (c) Zack Loveless.  All rights reserved.
//      
//      LICENSE TBA
//  </copyright>
// -----------------------------------------------------------------------------

namespace Lantea.IntegrationTests.Helpers
{
	using System;
	using System.Net;

	public static class IrcClientHelper
	{
		internal static Func<String, IPHostEntry> GetHostEntry;

		public static void foo()
		{
			// System.Net.Dns.GetHostEntry()
			// Dns.GetHostEntry()
		}
	}
}