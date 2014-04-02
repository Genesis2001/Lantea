// -----------------------------------------------------------------------------
//  <copyright file="Hello.cs" company="Zack Loveless">
//      Copyright (c) Zack Loveless.  All rights reserved.
//  </copyright>
// -----------------------------------------------------------------------------

namespace HelloModule
{
	using System;
	using System.ComponentModel.Composition;
	using Lantea.Common.Extensibility;
	using Lantea.Common.IO;

	[Export(typeof(IModule)), Module(Author = "Zack Loveless", Name = HelloName, Type = ModuleType.VENDOR, Version = HelloVersion)]
	public class Hello : ModuleCore
	{
		public const String HelloVersion = "1.0.1";
		public const String HelloName    = "Lantea Hello Module";

		[ImportingConstructor]
		public Hello([Import] IBotCore bot, [Import] Configuration config) : base(bot, config)
		{
		}

		#region Overrides of ModuleCore

		public override string Author
		{
			get { return "Zack Loveless"; }
		}

		public override string Name
		{
			get { return HelloName; }
		}

		public override string Version
		{
			get { return HelloVersion; }
		}

		public override ModuleType Type
		{
			get { return ModuleType.VENDOR; }
		}

		public override void Load()
		{
			Bot.Log.Info("Hello world!");

			base.Load();
		}

		#endregion
	}
}
