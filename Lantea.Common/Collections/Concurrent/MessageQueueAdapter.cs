// -----------------------------------------------------------------------------
//  <copyright file="MessageQueueAdapter.cs" company="Zack Loveless">
//      Copyright (c) Zack Loveless.  All rights reserved.
//      
//      LICENSE TBA
//  </copyright>
// -----------------------------------------------------------------------------

namespace Lantea.Common.Collections.Concurrent
{
	using System.Collections.Concurrent;
	using System.Collections.Generic;

	public class MessageQueueAdapter : IQueue<string>
	{
		private readonly ConcurrentQueue<string> queue;

		public MessageQueueAdapter(IEnumerable<string> messages)
		{
			queue = new ConcurrentQueue<string>(messages);
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
