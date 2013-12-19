// -----------------------------------------------------------------------------
//  <copyright file="VersionFixture.cs" company="Zack Loveless">
//      Copyright (c) Zack Loveless.  All rights reserved.
//      
//      LICENSE TBA
//  </copyright>
// -----------------------------------------------------------------------------

namespace Lantea.UnitTests.Fixtures
{
	using System;
	using System.Collections.Generic;
	using NUnit.Framework;
	using Version = Common.Modules.Version;

	// ReSharper disable InconsistentNaming
	// ReSharper disable PossibleNullReferenceException
	// ReSharper disable SuggestUseVarKeywordEverywhere

	[TestFixture]
	public class VersionFixture
	{
		private Version SUT;
		private const int major = 1;
		private const int minor = 2;
		private const int build = 5;
		private const int revision = 45;

		[SetUp]
		public void Setup()
		{
			SUT = new Version(major, minor);
		}

		[Test]
		public void Ctor_AcceptsMajorMinorNumbers()
		{
			const string expected = "1.2";
			string actual = SUT.ToString();

			Assert.AreEqual(expected, actual);
		}

		[Test]
		public void Major_ShouldBeSetFromCtor()
		{
			Assert.That(SUT.Major, Is.EqualTo(major));
		}

		[Test]
		public void Minor_ShouldBeSetFromCtor()
		{
			Assert.That(SUT.Minor, Is.EqualTo(minor));
		}

		[Test]
		public void Build_ShouldBeSetOptionallyFromCtor()
		{
			var optional = new Version(major, minor, build);

			Assert.That(optional.Build, Is.EqualTo(build));
		}

		[Test]
		public void Revision_ShouldBeOptionallySetFromCtor()
		{
			var optional = new Version(major, minor, build, revision);

			Assert.That(optional.Revision, Is.EqualTo(revision));
		}

		[Test]
		public void Version_ShouldImplementIEquatable()
		{
			Assert.That(SUT, Is.AssignableTo<IEquatable<Version>>());
		}

		[Test]
		public void Ctor_ShouldTakeInCollectionOfKeyValuePairs()
		{
			var expected = new Version(major, minor, build, revision);

			var dict = new Dictionary<string, int>
			           {
				           {"major", major},
				           {"minor", minor},
				           {"build", build},
				           {"revision", revision},
			           };

			var actual = new Version(dict);

			Assert.AreEqual(expected, actual);
		}

		[Test]
		[ExpectedException(typeof (InvalidOperationException))]
		public void Ctor_ShouldThrowExceptionIfMajorOrMinorDictionaryKeysAreNotPresent()
		{
			var dict = new Dictionary<string, int>
			           {
				           {"major", major},
			           };

			// Should throw exception
			var actual = new Version(dict);
		}

		[Test]
		public void Ctor_ShouldAllowOptionalBuildAndRevisionDictionaryKeysAndNotThrowException()
		{
			var dict = new Dictionary<string, int>
			           {
				           {"major", major},
				           {"minor", minor},
			           };

			var actual = new Version(dict);
		}

		[Test]
		public void ToString_ShouldJoinVersionPartsTogetherIntoOneString()
		{
			const string expected = "1.2.5.45";

			var v = new Version(major, minor, build, revision);
			var actual = v.ToString();

			Assert.AreEqual(expected, actual);
		}

		[Test]
		public void ToString_ShouldJoinVersionPartsTogetherThatAreNotNegativeOne()
		{
			const string expected = "1.2";

			var actual = SUT.ToString();

			Assert.AreEqual(expected, actual);
		}

		[Test]
		public void Version_ShouldBeAssignableFromString()
		{
			var expected = new Version(1, 2, 86);

			Version actual = "1.2.86";

			Assert.That(actual, Is.Not.Null);
			Assert.AreEqual(expected, actual);
		}

		[Test]
		public void CompareTo_ShouldReturnGreaterThanZeroWhenComparerVersionIsLessThanSUT()
		{
			var version = new Version(1, 3); // v0.2
			var actual = SUT.CompareTo(version); // 1.2 > 0.2

			Assert.That(actual, Is.LessThan(0));
			Assert.That(SUT, Is.LessThan(version));
		}

		[Test]
		public void CompareTo_ShouldReturnZeroWhenComparerIsEqualToSUT()
		{
			var version = new Version(major, minor); // v1.2
			var actual = SUT.CompareTo(version);

			Assert.That(actual, Is.EqualTo(0));
		}

		[Test]
		public void CompareTo_ShouldReturnNumberGreaterThanZeroWhenGreaterThanSUT()
		{
			var version = new Version(1, 1); // v1.1
			var actual = SUT.CompareTo(version);

			Assert.That(actual, Is.GreaterThan(0));
			Assert.That(SUT, Is.GreaterThan(version));
		}
	}

	// ReSharper enable InconsistentNaming
	// ReSharper enable PossibleNullReferenceException
	// ReSharper enable SuggestUseVarKeywordEverywhere
}
