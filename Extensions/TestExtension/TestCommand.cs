// -----------------------------------------------------------------------------
//  <copyright file="TestCommand.cs" company="Zack Loveless">
//      Copyright (c) Zack Loveless.  All rights reserved.
//  </copyright>
// -----------------------------------------------------------------------------

namespace TestExtension
{
	using System.ComponentModel.Composition;
	using Atlantis.Net.Irc;
	using Lantea.Core.Extensibility;

	public class TestCommand : CommandBase
	{
		[ImportingConstructor]
		public TestCommand([Import] IModule module) : base(module)
		{
		}

		#region Overrides of CommandBase

		public override string[] Triggers
		{
			get { return new[] { "!test" }; }
		}

		public override string Description
		{
			get { return "A simple test command."; }
		}

		public override bool CanExecute(object parameter)
		{
			PrefixList list = parameter as PrefixList;

			// TODO: implement >= and <= operators on prefixlist/prefixes.
			return list != null && list.HighestPrefix == '~';
		}

		public override void Execute(object parameter)
		{
			Parameter p = (Parameter)parameter;
			IrcClient c = module.Bot.Client;
			c.Message(p.Channel, "Hello {0}! You used !test with \"{1}\"", p.Source, string.Join(" ", p.Args));
		}

		#endregion
	}
}
