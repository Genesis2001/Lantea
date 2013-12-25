// -----------------------------------------------------------------------------
//  <copyright file="Version.cs" company="Zack Loveless">
//      Copyright (c) Zack Loveless.  All rights reserved.
//  </copyright>
// -----------------------------------------------------------------------------

namespace Lantea.Common.Modules
{
	using System;
	using System.Collections.Generic;
	using System.Linq;

	public class Version : IEquatable<Version>, IComparable<Version>
	{
		internal static readonly string[] VersionKeys = {"major", "minor", "build", "revision"};

		private static readonly Lazy<Version> EmptyInstance = new Lazy<Version>(true);

		public Version() : this(-1, -1)
		{
		}

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
				throw new InvalidOperationException("One or more version keys were ommitted in the incoming dictionary.",
					innerException);
			}
		}

		#region Public Properties

		public int Major { get; private set; }

		public int Minor { get; private set; }

		public int Build { get; private set; }

		public int Revision { get; private set; }

		#endregion

		#region Implementation of Null-object Pattern

		/// <summary>
		///     Represents the empty version. This field is read-only.
		/// </summary>
		public static Version Empty
		{
			get { return EmptyInstance.Value; }
		}

		#endregion

		#region Implementation of IComparable<Version>

		/// <summary>
		///     Compares the current object with another object of the same type.
		/// </summary>
		/// <returns>
		///     A value that indicates the relative order of the objects being compared. The return value has the following
		///     meanings: Value Meaning Less than zero This object is less than the <paramref name="other" /> parameter.Zero This
		///     object is equal to <paramref name="other" />. Greater than zero This object is greater than
		///     <paramref name="other" />.
		/// </returns>
		/// <param name="other">An object to compare with this object.</param>
		public int CompareTo(Version other)
		{
			if (Equals(other)) return 0; // We're equal.

			int i;

			if ((i = Major.CompareTo(other.Major)) != 0) return i;
			if ((i = Minor.CompareTo(other.Minor)) != 0) return i;
			if ((i = Build.CompareTo(other.Build)) != 0) return i;
			if ((i = Revision.CompareTo(other.Revision)) != 0) return i;

			return i;
		}

		#endregion

		#region Implementation of IEquatable<Version>

		/// <summary>
		///     Indicates whether the current object is equal to another object of the same type.
		/// </summary>
		/// <returns>
		///     true if the current object is equal to the <paramref name="other" /> parameter; otherwise, false.
		/// </returns>
		/// <param name="other">An object to compare with this object.</param>
		public bool Equals(Version other)
		{
			return other.Major == Major && other.Minor == Minor && other.Build == Build && other.Revision == Revision;
		}

		#endregion

		#region Overrides of Object

		/// <summary>
		///     Returns major string that represents the current object.
		/// </summary>
		/// <returns>
		///     A string that represents the current object.
		/// </returns>
		public override string ToString()
		{
			var arr = new[] {Major, Minor, Build, Revision};

			return string.Join(".", arr.Where(x => x > 0).Select(x => x.ToString()).ToArray());
		}

		#endregion

		#region Operators

		public static implicit operator Version(string value)
		{
			try
			{
				var parts = value.Split('.').Select(Int32.Parse).ToArray();
				var dict =
					parts.Select((val, index) => new KeyValuePair<String, Int32>(VersionKeys[index], val)).
						ToDictionary(x => x.Key, x => x.Value);

				return new Version(dict);
			}
			catch (Exception)
			{
				return null;
			}
		}

		public static bool operator <(Version left, Version right)
		{
			return left.CompareTo(right) < 0;
		}

		public static bool operator >(Version left, Version right)
		{
			return left.CompareTo(right) > 0;
		}

		public static bool operator <=(Version left, Version right)
		{
			if (left.Equals(right)) return true;
			return left.CompareTo(right) < 0;
		}

		public static bool operator >=(Version left, Version right)
		{
			if (left.Equals(right)) return true;
			return left.CompareTo(right) > 0;
		}

		#endregion
	}
}
