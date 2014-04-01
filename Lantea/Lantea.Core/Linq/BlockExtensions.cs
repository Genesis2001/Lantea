// -----------------------------------------------------------------------------
//  <copyright file="BlockExtensions.cs" company="Zack Loveless">
//      Copyright (c) Zack Loveless.  All rights reserved.
//  </copyright>
// -----------------------------------------------------------------------------

namespace Lantea.Core.Linq
{
	using System;
	using IO;

	public static class BlockExtensions
	{
		public static String GetString(this Block source, String property, String def = null)
		{
			return source.Get(property, def);
		}

		public static Int32 GetInt32(this Block source, String property, Int32 def = 0)
		{
			return source.Get(property, def);
		}

		public static Boolean GetBoolean(this Block source, String property, Boolean def = false)
		{
			return source.Get(property, def);
		}
	}
}
