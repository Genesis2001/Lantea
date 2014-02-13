// -----------------------------------------------------------------------------
//  <copyright file="NumericExtensions.cs" company="Zack Loveless">
//      Copyright (c) Zack Loveless.  All rights reserved.
//  </copyright>
// -----------------------------------------------------------------------------

namespace Atlantis.Linq
{
	using System;

	public static class NumericExtensions
	{
		private static readonly DateTime ORIGIN = new DateTime(1970, 1, 1, 0, 0, 0);

		public static DateTime ToDateTime(this double source)
		{
			return ORIGIN.AddSeconds(source);
		}

		public static double ToTimestamp(this DateTime source)
		{
			return Math.Floor((source - ORIGIN).TotalSeconds);
		}

		public static double ToDouble(this string source)
		{
			return Double.Parse(source);
		}
	}
}
