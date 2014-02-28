// -----------------------------------------------------------------------------
//  <copyright file="CommandBase.cs" company="Zack Loveless">
//      Copyright (c) Zack Loveless.  All rights reserved.
//  </copyright>
// -----------------------------------------------------------------------------

namespace Lantea.Core.Extensibility
{
	using System;
	using System.Collections.Generic;

	public abstract class CommandBase : ICommand
	{
		protected Predicate<object> condition;

		/// <summary>
		///     Initializes a new instance of the <see cref="T:System.Object" /> class.
		/// </summary>
		protected CommandBase()
		{
			Triggers = new List<string>();
		}

		#region Implementation of ICommand

		public IList<string> Triggers { get; private set; }

		public string Description { get; set; }

		public abstract bool CanExecute(object parameter);

		public abstract void Execute(object parameter);

		#endregion
	}
}
