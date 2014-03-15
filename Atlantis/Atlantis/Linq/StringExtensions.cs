// -----------------------------------------------------------------------------
//  <copyright file="StringExtensions.cs" company="Zack Loveless">
//      Copyright (c) Zack Loveless.  All rights reserved.
//  </copyright>
// -----------------------------------------------------------------------------

namespace Atlantis.Linq
{
	using System;
	using System.Text;

	public static partial class Extensions
	{
		public static bool EqualsIgnoreCase(this string source, string value)
		{
			return source.Equals(value, StringComparison.OrdinalIgnoreCase);
		}

		public static bool StartsWithIgnoreCase(this string source, string value)
		{
			return source.StartsWith(value, StringComparison.OrdinalIgnoreCase);
		}

		public static string TrimIfNotNull(this string source)
		{
			return source != null ? source.Trim() : string.Empty;
		}

		public static string TrimIfNotNull(this string source, params char[] trimChars)
		{
			return source != null ? source.Trim(trimChars) : string.Empty;
		}
	}
}
