// -----------------------------------------------------------------------------
//  <copyright file="MalformedConfigException.cs" company="Zack Loveless">
//      Copyright (c) Zack Loveless.  All rights reserved.
//  </copyright>
// -----------------------------------------------------------------------------

namespace Lantea.Common.IO
{
	using System;

	public class MalformedConfigException : Exception
	{
		public MalformedConfigException(string message) : base(message)
		{
		}

		public MalformedConfigException(string message, string fileName, int lineNumber)
			: base(string.Format("{0}: {1}:{2}", message, fileName, lineNumber))
		{
		}

		public MalformedConfigException(string message, Exception innerException) : base(message, innerException)
		{
		}
	}
}