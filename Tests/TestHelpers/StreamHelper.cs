﻿// -----------------------------------------------------------------------------
//  <copyright file="StreamHelper.cs" company="Zack Loveless">
//      Copyright (c) Zack Loveless.  All rights reserved.
//  </copyright>
// -----------------------------------------------------------------------------

namespace TestHelpers
{
	using System.IO;
	using System.Text;

	public static class StreamHelper
	{
		public static Stream AsStream(this string source)
		{
			byte[] buffer = Encoding.UTF8.GetBytes(source);

			return new MemoryStream(buffer);
		}
	}
}
