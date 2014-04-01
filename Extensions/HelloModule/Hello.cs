// -----------------------------------------------------------------------------
//  <copyright file="Hello.cs" company="Zack Loveless">
//      Copyright (c) Zack Loveless.  All rights reserved.
//  </copyright>
// -----------------------------------------------------------------------------

namespace HelloModule
{
	using System.ComponentModel.Composition;
	using Lantea.Common.Extensibility;
	using Lantea.Common.IO;

	[Export(typeof(IModule)), Module(Author = "Zack Loveless", Name = "Lantea Hello Module", Type = ModuleType.VENDOR, Version = "1.0")]
	public class Hello : ModuleCore
	{
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
			get { return "Lantea Hello Module"; }
		}

		public override string Version
		{
			get { return "1.0"; }
		}

		public override ModuleType Type
		{
			get { return ModuleType.VENDOR; }
		}

		public override void Load()
		{
			Bot.Log.Info("Hello world!");
		}

		#endregion
	}
}
