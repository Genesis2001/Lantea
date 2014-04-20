// -----------------------------------------------------------------------------
//  <copyright file="PathHelper.cs" company="Zack Loveless">
//      Copyright (c) Zack Loveless.  All rights reserved.
//  </copyright>
// -----------------------------------------------------------------------------

namespace Uptime
{
	using System;
	using System.IO;

	public static class PathHelper
	{
		public static String ExpandPath(String path)
		{
			if (String.IsNullOrEmpty(path))
			{
				throw new ArgumentNullException("path", "The specified path is null or empty.");
			}

			if (!path.StartsWith("~")) return path;

			String profile = Environment.GetFolderPath(Environment.SpecialFolder.UserProfile);
			path           = path.Substring(1);

			return Path.Combine(profile, path);
		}
	}
}
