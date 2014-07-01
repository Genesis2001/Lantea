// -----------------------------------------------------------------------------
//  <copyright file="PathExtensions.cs" company="Zack Loveless">
//      Copyright (c) Zack Loveless.  All rights reserved.
//  </copyright>
// -----------------------------------------------------------------------------

namespace Lantea.Common.Linq
{
    using System;
    using System.IO;

    public static class PathExtensions
    {
        /// <summary>
        /// Gets an absolute path of a given path. Returns null if it's not a valid Unc URI.
        /// </summary>
        /// <param name="source"></param>
        /// <returns></returns>
        /// <exception cref="System.ArgumentNullException" />
        public static string GetAbsolutePath(this string source)
        {
            if (source == null) throw new ArgumentNullException("source");

            Uri path;

            if (!Uri.TryCreate(source, UriKind.RelativeOrAbsolute, out path))
            {
                return null;
            }

            return !path.IsAbsoluteUri ? Path.GetFullPath(source) : path.AbsolutePath;
        }
    }
}
