// -----------------------------------------------------------------------------
//  <copyright file="Hello.cs" company="Zack Loveless">
//      Copyright (c) Zack Loveless.  All rights reserved.
//  </copyright>
// -----------------------------------------------------------------------------

namespace Hello
{
	using System;
	using System.ComponentModel.Composition;
	using Lantea.Common.Extensibility;
	using Lantea.Common.IO;

	[Export(typeof(IModule)), Module(Author = "Zack Loveless", Name = ModuleName, Type = ModuleType.VENDOR, Version = ModuleVersion)]
	public class Hello : ModuleCore
	{
		private const String ModuleVersion = "1.0.1";
		private const String ModuleName    = "Hello";
		private const String ModuleDesc    = "";

		[ImportingConstructor]
		public Hello([Import] IBotCore bot, [Import] Configuration config) : base(bot, config)
		{
		}

		#region Overrides of ModuleCore

		public override string Author
		{
			get { return "Zack Loveless"; }
		}

		public override string Description
		{
			get { return ModuleDesc; }
		}

		public override string Name
		{
			get { return ModuleName; }
		}

		public override string Version
		{
			get { return ModuleVersion; }
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
