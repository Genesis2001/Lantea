// -----------------------------------------------------------------------------
//  <copyright file="StringExtensions.cs" company="Zack Loveless">
//      Copyright (c) Zack Loveless.  All rights reserved.
//  </copyright>
// -----------------------------------------------------------------------------

namespace Lantea.Common.Linq
{
	using System;
	using System.Collections.Generic;
	using System.Text;
	using System.Text.RegularExpressions;

	public static partial class Extensions
	{
		// ReSharper disable InconsistentNaming
		private static readonly Dictionary<string, Regex> regexes = new Dictionary<string, Regex>();
		// ReSharper restore InconsistentNaming

		public static Match Match(this string source, string expression)
		{
			Regex r;

			if (regexes.TryGetValue(expression, out r))
			{
				return r.Match(expression);
			}

			r = new Regex(expression, RegexOptions.Compiled);
			regexes.Add(expression, r);

			return r.Match(expression);
		}
		
		/// <summary>
		/// Attempts a regular expression match on the source string using the expression provided.
		/// </summary>
		/// <param name="source"></param>
		/// <param name="expression"></param>
		/// <param name="match">The matched expression object containing the groups that were matched.</param>
		/// <returns>Returns True or False depending whether the match was successful.</returns>
		public static bool TryMatch(this string source, string expression, out Match match)
		{
			Regex r;
			if (regexes.TryGetValue(expression, out r))
			{
				match = r.Match(source);
				return match.Success;
			}

			r = new Regex(expression, RegexOptions.Compiled);
			regexes.Add(expression, r);

			match = r.Match(source);
			return match.Success;
		}

		public static bool TryMatches(this string source, string expression, out MatchCollection match)
		{
			Regex r;
			if (regexes.TryGetValue(expression, out r))
			{
				match = r.Matches(source);
				return match.Count > 0;
			}

			r = new Regex(expression, RegexOptions.Compiled);
			regexes.Add(expression, r);

			match = r.Matches(source);
			return match.Count > 0;
		}

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
	}
}
