// -----------------------------------------------------------------------------
//  <copyright file="RawMessageEventArgs.cs" company="Zack Loveless">
//      Copyright (c) Zack Loveless.  All rights reserved.
//  </copyright>
// -----------------------------------------------------------------------------

namespace Lantea.Core.Net.Irc
{
	using System;

	public class RawMessageEventArgs : EventArgs
	{
		public RawMessageEventArgs(string message)
		{
			Message = message;
		}

		public string Message { get; set; }
	}
}
