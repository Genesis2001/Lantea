// -----------------------------------------------------------------------------
//  <copyright file="RfcNumericEventArgs.cs" company="Zack Loveless">
//      Copyright (c) Zack Loveless.  All rights reserved.
//  </copyright>
// -----------------------------------------------------------------------------

namespace Lantea.Core.Net.Irc
{
	using System;

	public class RfcNumericEventArgs : EventArgs
	{
		public RfcNumericEventArgs(int numeric, string message)
		{
			Numeric = numeric;
			Message = message;
		}

		public int Numeric { get; private set; }

		public string Message { get; private set; }
	}
}
