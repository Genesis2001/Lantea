// -----------------------------------------------------------------------------
//  <copyright file="TestExtension.cs" company="Zack Loveless">
//      Copyright (c) Zack Loveless.  All rights reserved.
//  </copyright>
// -----------------------------------------------------------------------------

namespace TestExtension
{
	using Lantea.Core.Extensibility;
	using Lantea.Core.IO;

	[Module(ModuleType.EXTRA, "1.0")]
	public class TestExtension : ModuleBase
	{
		public TestExtension(IBotCore bot) : base(bot)
		{
		}

		#region Overrides of ModuleBase

		public override void Initialize()
		{
			throw new System.NotImplementedException();
		}

		public override void Rehash(Configuration config)
		{
			throw new System.NotImplementedException();
		}

		public override string Name
		{
			get { return "Test Extension"; }
		}

		public override string Author
		{
			get { return "Zack Loveless"; }
		}

		public override ModuleType Type
		{
			get { return ModuleType.EXTRA; }
		}

		public override string Version
		{
			get { return "1.0"; }
		}

		#endregion
	}

	public class TestCommand : CommandBase
	{
		#region Overrides of CommandBase

		public override bool CanExecute(object parameter)
		{
			return true;
		}

		public override void Execute(object parameter)
		{

		}

		#endregion
	}
}
