// -----------------------------------------------------------------------------
//  <copyright file="Extensions.cs" company="Zack Loveless">
//      Copyright (c) Zack Loveless.  All rights reserved.
//  </copyright>
// -----------------------------------------------------------------------------

namespace Uptime
{
	using System;
	using System.Text;

	public static class Extensions
	{
		public static String Prettify(this TimeSpan source)
		{
			if (source == null)
			{
				throw new ArgumentNullException("source", "The specified System.TimeSpan was null. :(");
			}

			// TODO: Internationalization this method.
			StringBuilder builder = new StringBuilder();

			if (source.Days > 0) builder.AppendFormat("{0} days", source.Days);
			if (source.Hours > 0) builder.AppendFormat(", {0} hours", source.Hours);
			if (source.Minutes > 0) builder.AppendFormat(", {0} minutes", source.Minutes);
			if (source.Seconds > 0) builder.AppendFormat(", {0} seconds", source.Seconds);

			return builder.ToString();
		}
	}
}
