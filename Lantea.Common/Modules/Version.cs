// -----------------------------------------------------------------------------
//  <copyright file="Version.cs" company="Zack Loveless">
//      Copyright (c) Zack Loveless.  All rights reserved.
//      
//      LICENSE TBA
//  </copyright>
// -----------------------------------------------------------------------------

namespace Lantea.Common.Modules
{
	using System;
	using System.Collections.Generic;
	using System.Linq;

	public class Version : IEquatable<Version>, IComparable<Version>
	{
		internal static readonly string[] VersionFormat = {"major", "minor", "build", "revision"};

		public Version(int major, int minor, int build = 0, int revision = 0)
		{
			Major = major;
			Minor = minor;
			Build = build;
			Revision = revision;
		}

		public Version(IDictionary<string, int> dict)
		{
			try
			{
				Major = dict["major"];
				Minor = dict["minor"];

				int build, revision;
				if (dict.TryGetValue("build", out build))
				{
					Build = build;
				}

				if (dict.TryGetValue("revision", out revision))
				{
					Revision = revision;
				}
			}
			catch (KeyNotFoundException innerException)
			{
				throw new InvalidOperationException("One or more version keys were ommitted in the incoming dictionary.", innerException);
			}
		}

		public int Major { get; private set; }

		public int Minor { get; private set; }

		public int Build { get; private set; }

		public int Revision { get; private set; }

		#region Implementation of IComparable<Version>

		/// <summary>
		/// Compares the current object with another object of the same type.
		/// </summary>
		/// <returns>
		/// A value that indicates the relative order of the objects being compared. The return value has the following meanings: Value Meaning Less than zero This object is less than the <paramref name="other"/> parameter.Zero This object is equal to <paramref name="other"/>. Greater than zero This object is greater than <paramref name="other"/>. 
		/// </returns>
		/// <param name="other">An object to compare with this object.</param>
		public int CompareTo(Version other)
		{
			throw new NotImplementedException();
		}

		#endregion

		#region Implementation of IEquatable<Version>

		/// <summary>
		/// Indicates whether the current object is equal to another object of the same type.
		/// </summary>
		/// <returns>
		/// true if the current object is equal to the <paramref name="other"/> parameter; otherwise, false.
		/// </returns>
		/// <param name="other">An object to compare with this object.</param>
		public bool Equals(Version other)
		{
			return other.Major == Major && other.Minor == Minor && other.Build == Build && other.Revision == Revision;
		}

		#endregion

		#region Overrides of Object

		/// <summary>
		/// Returns major string that represents the current object.
		/// </summary>
		/// <returns>
		/// A string that represents the current object.
		/// </returns>
		public override string ToString()
		{
			var arr = new[] {Major, Minor, Build, Revision};

			return string.Join(".", arr.Where(x => x > 0).Select(x => x.ToString()).ToArray());
		}

		#endregion

		public static implicit operator Version(string value)
		{
			try
			{
				var parts = value.Split('.').Select(Int32.Parse).ToArray();
				var dict = parts.Select((val, index) => new { key = VersionFormat[index], val }).ToDictionary(x => x.key, x => x.val);

				return new Version(dict);
			}
			catch (Exception)
			{
				return null;
			}
		}
	}
}
