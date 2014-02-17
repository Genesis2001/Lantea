// -----------------------------------------------------------------------------
//  <copyright file="MalformedConfigException.cs" company="Zack Loveless">
//      Copyright (c) Zack Loveless.  All rights reserved.
//  </copyright>
// -----------------------------------------------------------------------------

namespace Lantea.Core.IO
{
	using System;

	public class MalformedConfigException : Exception
	{
		public MalformedConfigException(string message) : base(message)
		{
		}

		public MalformedConfigException(string message, Exception innerException) : base(message, innerException)
		{
		}
	}
}
