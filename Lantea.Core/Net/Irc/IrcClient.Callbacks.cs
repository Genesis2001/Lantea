// -----------------------------------------------------------------------------
//  <copyright file="IrcClient.Callbacks.cs" company="Zack Loveless">
//      Copyright (c) Zack Loveless.  All rights reserved.
//  </copyright>
// -----------------------------------------------------------------------------

namespace Lantea.Core.Net.Irc
{
	using System;
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
				Send(string.Format("PONG {0}", toks[1])); // expedite the pong reply (avoid the queue)
			}

			int num;
			if (Int32.TryParse(toks[1], out num))
			{
				OnRawNumeric(num, input);
			}

			var handler = RawMessageEvent;
			if (handler != null) handler(this, new RawMessageEventArgs(input));
		}

		protected virtual void OnRawNumeric(int numeric, string message)
		{
			var handler = RfcNumericEvent;
			if (handler != null) handler(this, new RfcNumericEventArgs(numeric, message));
		}

		protected void ThreadWorkerCallback()
		{
			queueRunner = Task.Run(new Action(ThreadQueueCallback), cancellationTokenSource.Token);

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

		protected async void ThreadQueueCallback()
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
