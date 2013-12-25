// -----------------------------------------------------------------------------
//  <copyright file="IObservableClass.cs" company="Zack Loveless">
//      Copyright (c) Zack Loveless.  All rights reserved.
//  </copyright>
// -----------------------------------------------------------------------------

namespace Lantea.Common
{
	using System.ComponentModel;

	public interface IObservableClass : INotifyPropertyChanged, INotifyPropertyChanging
	{
		void NotifyPropertyChanged(string propertyName);

		void NotifyPropertyChanging(string propertyName);
	}
}
