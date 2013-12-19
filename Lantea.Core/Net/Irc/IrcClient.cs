// -----------------------------------------------------------------------------
//  <copyright file="IrcClient.cs" company="Zack Loveless">
//      Copyright (c) Zack Loveless.  All rights reserved.
//      
//      LICENSE TBA
//  </copyright>
// -----------------------------------------------------------------------------

namespace Lantea.Core.Net.Irc
{
	using System;
	using System.Net;

	public class IrcClient
	{
		public static Func<String, IPHostEntry> GetHostEntry;

		public IrcClient()
		{
			GetHostEntry = Dns.GetHostEntry;
		}

		#region Properties

		public string Host { get; set; }

		public string Nick { get; set; }

		public bool IsInitialized 
		{
			get
			{
				// ReSharper disable ReplaceWithSingleAssignment.True
				var val = true;

				if (string.IsNullOrEmpty(Nick)) val = false;
				else if (string.IsNullOrEmpty(Host) || GetHostEntry(Host) == null) val = false;
				
				return val;
				// ReSharper restore ReplaceWithSingleAssignment.True
			}
		}

		#endregion

		#region Events

		// 

		#endregion
	}
}
