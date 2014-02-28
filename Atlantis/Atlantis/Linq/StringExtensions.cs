﻿// -----------------------------------------------------------------------------
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
		public static StringBuilder AppendFormatLine(this StringBuilder source, string format, params object[] args)
		{
			source.AppendFormat(format, args);

			return source.Append(Environment.NewLine);
		}

		public static bool EqualsIgnoreCase(this string source, string value)
		{
			return source.Equals(value, StringComparison.InvariantCultureIgnoreCase);
		}

		public static bool StartsWithIgnoreCase(this string source, string value)
		{
			return source.StartsWith(value, StringComparison.InvariantCultureIgnoreCase);
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
