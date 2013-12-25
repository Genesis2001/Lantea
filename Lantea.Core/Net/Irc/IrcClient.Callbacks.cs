// -----------------------------------------------------------------------------
//  <copyright file="IrcClient.Callbacks.cs" company="Zack Loveless">
//      Copyright (c) Zack Loveless.  All rights reserved.
//  </copyright>
// -----------------------------------------------------------------------------

namespace Lantea.Core.Net.Irc
{
	using System;
	using System.Threading;
	using System.Threading.Tasks;
	using Common.Linq;

	public partial class IrcClient
	{
		#region Methods

		protected virtual void OnDataReceived(string input)
		{
			var toks = input.Split(' ');

			if (toks[0].EqualsIgnoreCase("ping"))
			{
				Send(string.Format("PONG {0}", toks[1]));
			}

			int num;
			if (Int32.TryParse(toks[1], out num))
			{
				OnRawNumeric(num, input);
			}
		}

		protected virtual void OnRawNumeric(int numeric, string message)
		{
			var handler = RfcNumericEvent;
			if (handler != null) handler(this, new RfcNumericEventArgs(numeric, message));
		}

		protected void ThreadWorkerCallback()
		{
			/*if (o != null)
			{
				RfcNumericEvent += (s, e) =>
				                   {
					                   if (e.Numeric.Equals(001))
					                   {
						                   ((EventWaitHandle)o).Set();
					                   }
				                   };
			}*/

//			queueThread.Start();
			// queueRunner.Start();

			var registration = BuildRegistrationPacket();
			Send(registration);

			while (client != null && client.Connected)
			{
				if (!client.DataAvailable) continue;

				while (!client.EndOfStream)
				{
					var line = client.ReadLine().Trim();

					if (!string.IsNullOrEmpty(line))
					{
						OnDataReceived(line);
					}
				}
			}
		}

		protected async void ThreadQueueCallback(object obj)
		{
			while (client != null && client.Connected)
			{
				if (messageQueue.Count > 0)
				{
					Send(messageQueue.Pop());
				}

				await Task.Delay(QueueInteval);
			}
		}

		#endregion
	}
}
