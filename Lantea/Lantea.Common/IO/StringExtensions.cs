// -----------------------------------------------------------------------------
//  <copyright file="StringExtensions.cs" company="Zack Loveless">
//      Copyright (c) Zack Loveless.  All rights reserved.
//  </copyright>
// -----------------------------------------------------------------------------

namespace Lantea.Common.IO
{
	using System;

	public static class Extensions
	{
		public static Boolean ToBoolean(this String source)
		{
			if (source == null)
			{
				throw new ArgumentNullException("source");
			}

			return source.Equals("yes", StringComparison.OrdinalIgnoreCase);
		}
	}
}
