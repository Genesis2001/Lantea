// -----------------------------------------------------------------------------
//  <copyright file="ConsoleLog.cs" company="Zack Loveless">
//      Copyright (c) Zack Loveless.  All rights reserved.
//  </copyright>
// -----------------------------------------------------------------------------

namespace Lantea.Core
{
	using System;
	using Atlantis.IO;

	public delegate void ConsoleWriteDelegate(String arg0);

	public class ConsoleLog : LogBaseClass
	{
		private readonly ConsoleWriteDelegate console;

		/// <summary>
		/// Initializes a new instance of the <see cref="T:ConsoleLog"/> class.
		/// </summary>
		public ConsoleLog(ConsoleWriteDelegate console)
		{
			this.console = console;
		}

		public void Write(String format, params object[] args)
		{
			console(String.Format(format, args));
		}

		public void WriteLine(String format, params object[] args)
		{
			console(String.Format(format, args));
			console("\n");
		}

		#region Overrides of LogBaseClass

		protected override void Dispose(Boolean disposing)
		{
			// nom
		}
		
		protected override void Write(LogThreshold threshold, String format, params object[] args)
		{
			if (Threshold.HasFlag(threshold))
			{
				BuildLogMessage(threshold, format, args);

				Write(message.ToString());
			}
		}

		#endregion
	}
}
