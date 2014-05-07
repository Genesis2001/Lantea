// -----------------------------------------------------------------------------
//  <copyright file="IIoCContainer.cs" company="Zack Loveless">
//      Copyright (c) Zack Loveless.  All rights reserved.
//  </copyright>
// -----------------------------------------------------------------------------

namespace Lantea.Common
{
	public interface IIoCContainer
	{
		void RegisterContract<T>(T item);

		T RetrieveContract<T>();
	}
}
