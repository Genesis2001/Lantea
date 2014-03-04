﻿// -----------------------------------------------------------------------------
//  <copyright file="JenkinsTestFixture.cs" company="Zack Loveless">
//      Copyright (c) Zack Loveless.  All rights reserved.
//  </copyright>
// -----------------------------------------------------------------------------

namespace Lantea.UnitTests
{
	using NUnit.Framework;

	// ReSharper disable InconsistentNaming
	// ReSharper disable PossibleNullReferenceException

	[TestFixture]
	public class JenkinsTestFixture
	{
		[Test]
		public void Jenkins_TestShouldFail()
		{
			Assert.Fail();
		}

		[Test]
		public void Jenkins_TestShouldBeInconclusive()
		{
			Assert.Inconclusive();
		}

		[Test]
		public void Jenkins_TestShouldPass()
		{
			Assert.Pass();
		}

		[Test]
		public void Jenkins_TestShouldBeIgnored()
		{
			Assert.Ignore();
		}
	}

	// ReSharper enable InconsistentNaming
	// ReSharper enable PossibleNullReferenceException
}
