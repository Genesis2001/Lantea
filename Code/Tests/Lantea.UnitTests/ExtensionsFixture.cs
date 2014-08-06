// -----------------------------------------------------------------------------
//  <copyright file="ExtensionsFixture.cs" company="Zack Loveless">
//      Copyright (c) Zack Loveless.  All rights reserved.
//  </copyright>
// -----------------------------------------------------------------------------

namespace Lantea.UnitTests
{
	using System;
	using System.Diagnostics.CodeAnalysis;
	using Atlantis.Linq;
	using NUnit.Framework;

	[ExcludeFromCodeCoverage]
	public class ExtensionsFixture
	{
		[Test, Category("Atlantis Extension Tests")]
		public void EqualsIgnoreCase_WithLowerString_ShouldEqualSameStringInUpperCase()
		{
			const string upperCase = "STRING";
			const string lowerCase = "string";

			bool actual         = lowerCase.EqualsIgnoreCase(upperCase);
			
			Assert.That(actual, Is.True);
		}

		[Test, Category("Atlantis Extension Tests")]
		public void StartsWithIgnoreCase_WithLowerString_ShouldReturnTrueWhenComparedWithUpperString()
		{
			const string testString = "STRTest";
			const string comparer   = "str";

			bool actual = testString.StartsWithIgnoreCase(comparer);

			Assert.That(actual, Is.True);
		}

		[Test, Category("Atlantis Extension Tests")]
		public void StringMatches_WithRegularExpression_ShouldReturnTrueForStringMatching()
		{
			const string testString = "1234";
			const string expression = @"\d";

			bool actual = testString.Matches(expression);

			Assert.That(actual, Is.True);
		}

		[Test, Category("Atlantis Extension Tests")]
		[ExpectedException(typeof (FormatException))]
		public void StringToDouble_WithNonDouble_ShouldThrowFormatException()
		{
			const string testString = "this is not a number";

			double expected         = testString.ToDouble();
		}

		[Test, Category("Atlantis Extension Tests")]
		public void StringToDouble_WithDoubleAsString_ShouldReturnStringAsDouble()
		{
			const string testString = "10.0";

			double expected         = testString.ToDouble();

			Assert.That(expected, Is.EqualTo(10.0));
		}

		[Test, Category("Atlantis Extension Tests")]
		public void ToDateTime_WithOneWeekSeconds_ShouldReturnCorrectDateTime()
		{
			// 604800
			const double value = 604800.0;
			
			DateTime actual    = value.ToDateTime();
			DateTime expected  = new DateTime(1970, 1, 8, 0, 0, 0);

			Assert.That(actual, Is.EqualTo(expected));
		}

		[Test, Category("Atlantis Extension Tests")]
		[ExpectedException(typeof (ArgumentException))]
		public void ToDateTime_WithNegativeValue_ShouldThrowArgumentException()
		{
			const double value = -1.0;

			DateTime actual    = value.ToDateTime();
		}

	}
}
