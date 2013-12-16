// -----------------------------------------------------------------------------
//  <copyright file="ModuleFixture.cs" company="Zack Loveless">
//      Copyright (c) Zack Loveless.  All rights reserved.
//      
//      LICENSE TBA
//  </copyright>
// -----------------------------------------------------------------------------

namespace Lantea.UnitTests
{
	using System;
	using Common.Modules;
	using Mocks;
	using NUnit.Framework;
	using Version = Common.Modules.Version;

	// ReSharper disable InconsistentNaming
	// ReSharper disable PossibleNullReferenceException

	[TestFixture]
	public class ModuleFixture
	{
//		private Mock<IModule> MOCK;
		private IModule SUT;

		[SetUp]
		public void Setup()
		{
			SUT = new MockModule();
		}

		[Test]
		public void Constructor_ImplementsIModuleMeta()
		{
			Assert.That(SUT, Is.AssignableTo<IModuleMeta>());
		}

		[Test]
		public void Author_IsNotNullOrEmpty()
		{
			Assert.That(SUT.Author, Is.Not.Null);
			Assert.That(SUT.Author, Is.Not.Empty);
		}

		[Test]
		public void Version_IsNotNullOrEmpty()
		{
			Assert.That(SUT.Version, Is.Not.Null);
			Assert.That(SUT.Version, Is.Not.EqualTo(Version.Empty));
		}

		[Test]
		public void IsEnabled_ShouldBeTrueWhenSetTrue()
		{
			SUT.IsEnabled = true;

			Assert.That(SUT.IsEnabled, Is.True);
		}

		[Test]
		public void Load_ShouldSetIsLoadedToTrue()
		{
			SUT.Load();

			Assert.That(SUT.IsLoaded, Is.True);
		}

		[Test]
		[ExpectedException(typeof (InvalidOperationException))]
		public void Load_CallingLoadTwice_ThrowsException()
		{
			SUT.Load();

			SUT.Load();
		}

		[Test]
		public void Unload_AfterLoad_ShouldSetIsLoadedToFalse()
		{
			SUT.Load();
			Assert.That(SUT.IsLoaded, Is.True);

			SUT.Unload();
			Assert.That(SUT.IsLoaded, Is.False);
		}

		[Test]
		[ExpectedException(typeof (InvalidOperationException))]
		public void Unload_WhenNotLoaded_ShouldThrowException()
		{
			SUT.Unload();
		}
	}

	// ReSharper enable InconsistentNaming
	// ReSharper enable PossibleNullReferenceException
}
