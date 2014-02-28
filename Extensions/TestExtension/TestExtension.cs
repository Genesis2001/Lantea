// -----------------------------------------------------------------------------
//  <copyright file="TestExtension.cs" company="Zack Loveless">
//      Copyright (c) Zack Loveless.  All rights reserved.
//  </copyright>
// -----------------------------------------------------------------------------

namespace TestExtension
{
	using System.Collections.Generic;
	using Lantea.Core.Extensibility;
	using Lantea.Core.IO;

	[Module(ModuleType.EXTRA, "1.0")]
	public class TestExtension : IModule
	{
		public TestExtension()
		{
			Commands = new List<ICommand>();
		}

		#region Implementation of IModuleAttribute

		public string Name
		{
			get { return "Test Extension"; }
		}

		public string Author
		{
			get { return "Zack Loveless"; }
		}

		public ModuleType Type
		{
			get { return ModuleType.EXTRA; }
		}

		public string Version
		{
			get { return "1.0"; }
		}

		#endregion

		#region Implementation of IModule

		public IBotCore Bot { get; set; }

		public IList<ICommand> Commands { get; private set; }

		public void Initialize()
		{
			Commands.Add(new TestCommand());
		}

		public void Rehash(Configuration config)
		{
			throw new System.NotImplementedException();
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
