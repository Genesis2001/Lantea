// -----------------------------------------------------------------------------
//  <copyright file="Uptime.cs" company="Zack Loveless">
//      Copyright (c) Zack Loveless.  All rights reserved.
//  </copyright>
// -----------------------------------------------------------------------------

namespace Uptime
{
	using System;
	using System.ComponentModel.Composition;
	using Lantea.Common.Extensibility;
	using Lantea.Common.IO;

	[Module(Author = "Zack Loveless", Name = ModuleName, Type = ModuleType.VENDOR, Version = ModuleVersion)]
	public class Uptime : ModuleCore
	{
		private const String ModuleName = "Service Uptime Module";
		private const String ModuleVersion = "1.0";

		[ImportingConstructor]
		public Uptime([Import] IBotCore bot, [Import] Configuration config) : base(bot, config)
		{
		}

		#region Overrides of ModuleCore

		public override string Author
		{
			get { return "Zack Loveless"; }
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

		#endregion

		#region Overrides of ModuleCore

		public override void Load()
		{
			Block uptimeBlock = Config.GetBlock("uptime");

			if (uptimeBlock != null)
			{
				for (Int32 i = 0; i < uptimeBlock.CountBlock("server"); ++i)
				{
					// todo.
				}
			}

			base.Load();
		}

		#endregion
	}
}
