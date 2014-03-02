// -----------------------------------------------------------------------------
//  <copyright file="CommandBase.cs" company="Zack Loveless">
//      Copyright (c) Zack Loveless.  All rights reserved.
//  </copyright>
// -----------------------------------------------------------------------------

namespace Lantea.Core.Extensibility
{
	public abstract class CommandBase : ICommand
	{
		protected readonly IModule module;

		protected CommandBase(IModule module)
		{
			this.module = module;
		}

		#region Implementation of ICommand

		public abstract string[] Triggers { get; }

		public abstract string Description { get; }

		public abstract bool CanExecute(object parameter);

		public abstract void Execute(object parameter);

		#endregion
	}
}
