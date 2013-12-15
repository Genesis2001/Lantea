// -----------------------------------------------------------------------------
//  <copyright file="IQueue.cs" company="Zack Loveless">
//      Copyright (c) Zack Loveless.  All rights reserved.
//      
//      LICENSE TBA
//  </copyright>
// -----------------------------------------------------------------------------

namespace Lantea.Common.Collections
{
	public interface IQueue<T> where T : class
	{
		T Pop();

		void Push(T item);
	}
}
