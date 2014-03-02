// -----------------------------------------------------------------------------
//  <copyright file="TestExtension.cs" company="Zack Loveless">
//      Copyright (c) Zack Loveless.  All rights reserved.
//  </copyright>
// -----------------------------------------------------------------------------

namespace TestExtension
{
	using System;
	using System.ComponentModel.Composition;
	using Lantea.Core.Extensibility;

	[Module(ModuleType.EXTRA, "1.0", Name = "Example Extension", Author = "Zack Loveless")]
	public class TestExtension : ModuleBase
	{
		[ImportingConstructor]
		public TestExtension([Import] IBotCore bot) : base(bot)
		{
		}

		#region Overrides of ModuleBase

		public override void Initialize()
		{
			Console.WriteLine("Test Extension loaded.");
		}
		
		#endregion
	}
}
