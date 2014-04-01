// -----------------------------------------------------------------------------
//  <copyright file="ConfigurationFixture.cs" company="Zack Loveless">
//      Copyright (c) Zack Loveless.  All rights reserved.
//  </copyright>
// -----------------------------------------------------------------------------
namespace Lantea.UnitTests
{
	using System.Diagnostics.CodeAnalysis;
	using System;
	using System.Collections.Generic;
	using System.IO;
	using Common.IO;
	using NUnit.Framework;
	using TestHelpers;

	// ReSharper disable InconsistentNaming
	// ReSharper disable PossibleNullReferenceException

	[TestFixture]
	[ExcludeFromCodeCoverage]
	public class ConfigurationFixture
	{
		private Configuration SUT;

		[SetUp]
		public void Setup()
		{
			SUT = new Configuration();
		}

		[Test, Category("Configuration Tests")]
		public void CountBlock_WithSingleEmptyBlock_ShouldReturnOne()
		{
			Stream stream = ConfigurationStrings.SingleEmptyBlock.AsStream();

			SUT.Load(stream);

			const int expected = 1;
			int result         = SUT.CountBlock("block");

			Assert.That(result, Is.EqualTo(expected));
		}

		[Test, Category("Configuration Tests")]
		public void CountBlock_WithNoBlocks_ShouldReturnZero()
		{
			Stream stream = ConfigurationStrings.EmptyFile.AsStream();

			SUT.Load(stream);

			const int expected = 0;
			int result         = SUT.CountBlock("block");

			Assert.That(result, Is.EqualTo(expected));
		}

		[Test, Category("Configuration Tests")]
		public void CountBlock_WithNoMatchingBlocks_ShouldReturnZero()
		{
			Stream stream = ConfigurationStrings.SingleEmptyBlock.AsStream();

			SUT.Load(stream);

			const int expected = 0;
			int result         = SUT.CountBlock("block2");

			Assert.That(result, Is.EqualTo(expected));
		}


		[Test, Category("Configuration Tests")]
		public void CountBlock_WithThreeIdenticalBlocks_ShouldReturnThree()
		{
			Stream stream = ConfigurationStrings.ThreeEmptyBlocks.AsStream();

			SUT.Load(stream);

			const int expected = 3;
			int result         = SUT.CountBlock("block");

			Assert.That(result, Is.EqualTo(expected));
		}

		[Test, Category("Configuration Tests")]
		[ExpectedException(typeof(ArgumentNullException))]
		public void CountBlock_WithNullArgument_ShouldThrowArgumentNullException()
		{
			Stream stream = ConfigurationStrings.SingleEmptyBlock.AsStream();

			SUT.Load(stream);

			SUT.CountBlock(null); // exception
		}

		[Test, Category("Configuration Tests")]
		public void CountBlock_WithUnloadedConfiguration_ShouldReturnZero()
		{
			const int expected = 0;
			int result         = SUT.CountBlock("block");

			Assert.That(result, Is.EqualTo(expected));
		}

		[Test, Category("Configuration Tests")]
		[ExpectedException(typeof (ArgumentNullException))]
		public void GetBlock_WithNullArgument_ShouldThrowArgumentNullException()
		{
			Stream stream = ConfigurationStrings.SingleEmptyBlock.AsStream();

			SUT.Load(stream);

			SUT.GetBlock(null); // exception
		}

		[Test, Category("Configuration Tests")]
		[ExpectedException(typeof (KeyNotFoundException))]
		public void GetBlock_WithNonExistantBlockName_ShouldThrowKeyNotFoundException()
		{
			SUT.GetBlock("none"); // exception
		}

		[Test, Category("Configuration Tests")]
		public void GetBlock_WithBlockAndAttributes_ShouldReturnListOfAttributes()
		{
			Stream stream = ConfigurationStrings.SingleBlockWithNameProperty.AsStream();

			SUT.Load(stream);

			Block block           = SUT.GetBlock("block");

			const string expected = "some block";
			string actual         = block.Get<String>("name");

			Assert.That(actual, Is.EqualTo(expected));
		}

		[Test, Category("Configuration Tests")]
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

		[Test, Category("Github Issue #12"), Category("Configuration Tests")]
		public void GetBlock_WithNestedBlock_ShouldGetNestedBlock()
		{
			Stream stream = ConfigurationStrings.BlockWithinBlock.AsStream();

			SUT.Load(stream);

			Block blockA = SUT.GetBlock("blockA");
			Assert.That(blockA, Is.Not.Null);

			Block blockB = blockA.GetBlock("blockB");
			Assert.That(blockB, Is.Not.Null);
		}

		[Test, Category("Github Issue #4"), Category("Configuration Tests")]
		public void Load_WithSingleLineBlock_ShouldLoadMatchingBlock()
		{
			Stream stream = ConfigurationStrings.SingleLineBlock.AsStream();

			SUT.Load(stream);

			Block actual = SUT.GetBlock("block");
			Assert.That(actual, Is.Not.Null);

			const String expectedName = "hello world";
			String actualName         = actual.Get<String>("name");
			Assert.That(actualName, Is.EqualTo(expectedName));
		}
	}

	// ReSharper enable InconsistentNaming
	// ReSharper enable PossibleNullReferenceException
}
