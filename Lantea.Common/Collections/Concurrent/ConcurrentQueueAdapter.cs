// -----------------------------------------------------------------------------
//  <copyright file="ConcurrentQueueAdapter.cs" company="Zack Loveless">
//      Copyright (c) Zack Loveless.  All rights reserved.
//      
//      LICENSE TBA
//  </copyright>
// -----------------------------------------------------------------------------

namespace Lantea.Common.Collections.Concurrent
{
	using System.Collections.Concurrent;
	using System.Collections.Generic;

	/// <summary>
	/// Represents a thread-safe first in-first out (FIFO) collection. Encapsulates a ConcurrentQueue&lt;String&gt;
	/// </summary>
	public class ConcurrentQueueAdapter : IQueue<string>
	{
		private readonly ConcurrentQueue<string> queue;

		public ConcurrentQueueAdapter(IEnumerable<string> items)
		{
			queue = new ConcurrentQueue<string>(items);
		}

		#region Implementation of IQueue<string>

		public string Pop()
		{
			string result;

			return queue.TryDequeue(out result) ? result : null;
		}

		public void Push(string item)
		{
			queue.Enqueue(item);
		}

		#endregion
	}
}
