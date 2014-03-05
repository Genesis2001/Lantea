// -----------------------------------------------------------------------------
//  <copyright file="ConfigurationFixture.cs" company="Zack Loveless">
//      Copyright (c) Zack Loveless.  All rights reserved.
//  </copyright>
// -----------------------------------------------------------------------------
namespace Lantea.UnitTests
{
	// ReSharper disable InconsistentNaming
	// ReSharper disable PossibleNullReferenceException

	using System;
	using System.Collections.Generic;
	using System.IO;
	using Core.IO;
	using NUnit.Framework;
	using TestHelpers;

	[TestFixture]
	public class ConfigurationFixture
	{
		private Configuration SUT;

		[SetUp]
		public void Setup()
		{
			SUT = new Configuration();
		}

		[Test]
		public void CountBlock_WithSingleEmptyBlock_ShouldReturnOne()
		{
			Stream stream = ConfigurationStrings.SingleEmptyBlock.AsStream();

			SUT.Load(stream);

			const int expected = 1;
			int result         = SUT.CountBlock("block");

			Assert.That(result, Is.EqualTo(expected));
		}

		[Test]
		public void CountBlock_WithNoBlocks_ShouldReturnZero()
		{
			Stream stream = ConfigurationStrings.EmptyFile.AsStream();

			SUT.Load(stream);

			const int expected = 0;
			int result         = SUT.CountBlock("block");

			Assert.That(result, Is.EqualTo(expected));
		}

		[Test]
		public void CountBlock_WithNoMatchingBlocks_ShouldReturnZero()
		{
			Stream stream = ConfigurationStrings.SingleEmptyBlock.AsStream();

			SUT.Load(stream);

			const int expected = 0;
			int result         = SUT.CountBlock("block2");

			Assert.That(result, Is.EqualTo(expected));
		}


		[Test]
		public void CountBlock_WithThreeIdenticalBlocks_ShouldReturnThree()
		{
			Stream stream = ConfigurationStrings.ThreeEmptyBlocks.AsStream();

			SUT.Load(stream);

			const int expected = 3;
			int result         = SUT.CountBlock("block");

			Assert.That(result, Is.EqualTo(expected));
		}

		[Test]
		[ExpectedException(typeof(ArgumentNullException))]
		public void CountBlock_WithNullArgument_ShouldThrowArgumentNullException()
		{
			Stream stream = ConfigurationStrings.SingleEmptyBlock.AsStream();

			SUT.Load(stream);

			SUT.CountBlock(null); // exception
		}

		[Test]
		public void CountBlock_WithUnloadedConfiguration_ShouldReturnZero()
		{
			const int expected = 0;
			int result         = SUT.CountBlock("block");

			Assert.That(result, Is.EqualTo(expected));
		}

		[Test]
		[ExpectedException(typeof (ArgumentNullException))]
		public void GetBlock_WithNullArgument_ShouldThrowArgumentNullException()
		{
			Stream stream = ConfigurationStrings.SingleEmptyBlock.AsStream();

			SUT.Load(stream);

			SUT.GetBlock(null); // exception
		}

		[Test]
		[ExpectedException(typeof (KeyNotFoundException))]
		public void GetBlock_WithNonExistantBlockName_ShouldThrowKeyNotFoundException()
		{
			SUT.GetBlock("none"); // exception
		}

		[Test]
		public void GetBlock_WithExistingBlock_ShouldReturnMatchingBlock()
		{
			Stream stream = ConfigurationStrings.SingleBlockWithNameProperty.AsStream();

			SUT.Load(stream);

			const string expected = "some block";
			Block block           = SUT.GetBlock("block");

			Assert.That(block, Is.Not.Null);

			string result         = block.Get<String>("name");

			Assert.That(result, Is.EqualTo(expected));
		}
	}

	// ReSharper enable InconsistentNaming
	// ReSharper enable PossibleNullReferenceException
}
