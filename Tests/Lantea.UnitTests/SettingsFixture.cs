// -----------------------------------------------------------------------------
//  <copyright file="SettingsFixture.cs" company="Zack Loveless">
//      Copyright (c) Zack Loveless.  All rights reserved.
//      
//      LICENSE TBA
//  </copyright>
// -----------------------------------------------------------------------------

namespace Lantea.UnitTests
{
	using Common;
	using Helpers;
	using NUnit.Framework;
	using Should;

	// ReSharper disable InconsistentNaming
	// ReSharper disable PossibleNullReferenceException

	[TestFixture]
	public class SettingsFixture
	{
		[Test]
		public void LoadFrom_StringPath_IsLoadedIsTrue()
		{
			var settings = Settings.LoadFrom("settings.xml");

			settings.ShouldNotBeNull();
			settings.IsLoaded.ShouldBeTrue();
		}

		[Test]
		public void LoadFrom_InputStream_IsLoadedIsTrue()
		{
			var stream = SettingsFixtureHelper.GetMockFileStream();
			var settings = Settings.LoadFrom(stream);

			settings.ShouldNotBeNull();
			settings.IsLoaded.ShouldBeTrue();
		}

		[Test]
		public void GetValue_WithValidXPathString_ReturnsStringValueCorrespondingToXPath()
		{
			Assert.Inconclusive("Test incomplete: postponed.");

			var stream = SettingsFixtureHelper.GetMockFileStream();
			var settings = Settings.LoadFrom(stream);

			var result = settings.GetValue("/Core/Connection/@Host");
			Assert.That(result, Is.EqualTo("irc.unifiedtech.org"));
		}
	}

	// ReSharper enable InconsistentNaming
	// ReSharper enable PossibleNullReferenceException
}
