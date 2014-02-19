// -----------------------------------------------------------------------------
//  <copyright file="StringHelpers.cs" company="Zack Loveless">
//      Copyright (c) Zack Loveless.  All rights reserved.
//  </copyright>
// -----------------------------------------------------------------------------
namespace Lantea.UnitTests.Helpers
{
	using System.IO;
	using System.Text;

	public static class StringHelpers
	{
		public static Stream AsReadOnlyStream(this string source)
		{
			byte[] data = Encoding.UTF8.GetBytes(source);

			return new MemoryStream(data, false) {Position = 0};
		}

		public static Stream AsStream(this string source)
		{
			byte[] data = Encoding.UTF8.GetBytes(source);

			return new MemoryStream(data) {Position = 0};
		}
	}
}
