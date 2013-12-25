// -----------------------------------------------------------------------------
//  <copyright file="IQueue.cs" company="Zack Loveless">
//      Copyright (c) Zack Loveless.  All rights reserved.
//  </copyright>
// -----------------------------------------------------------------------------

namespace Lantea.Common.Collections
{
	public interface IQueue<T> where T : class
	{
		int Count { get; }

		T Pop();

		void Push(T item);
	}
}
