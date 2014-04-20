// -----------------------------------------------------------------------------
//  <copyright file="MissingRequiredPropertyException.cs" company="Zack Loveless">
//      Copyright (c) Zack Loveless.  All rights reserved.
//  </copyright>
// -----------------------------------------------------------------------------

namespace Lantea.Common.IO
{
	using System;

	public class MissingRequiredPropertyException : ArgumentException
	{
		public MissingRequiredPropertyException()
		{
		}

		public MissingRequiredPropertyException(String message) : base(message)
		{
		}

		public MissingRequiredPropertyException(String message, Exception innerException) : base(message, innerException)
		{
		}

		public MissingRequiredPropertyException(String message, String property) : base(message, property)
		{
		}

		public MissingRequiredPropertyException(String message, String property, Exception innerException)
			: base(message, property, innerException)
		{
		}
	}
}
