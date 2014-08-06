﻿// -----------------------------------------------------------------------------
//  <copyright file="LogStream.cs" company="Zack Loveless">
//      Copyright (c) Zack Loveless.  All rights reserved.
//  </copyright>
// -----------------------------------------------------------------------------

namespace Lantea.Common.IO
{
	using System;
	using System.IO;
	using System.Linq;
	using System.Text;
	using Atlantis.IO;

	public class LogStream<T> : ILog where T : Stream
	{
		private readonly T stream;

		public LogStream(T stream)
		{
			this.stream = stream;
		}

		private void Write(LogThreshold threshold, string format, params object[] args)
		{
			if (!Threshold.HasFlag(threshold)) return;

			var sb = new StringBuilder();
			sb.Append(threshold.ToString().ToUpper());
			sb.Append(' ');

			if (PrefixLog)
			{
				if (!string.IsNullOrEmpty(Prefix)) sb.Append(Prefix);
				else sb.AppendFormat("{0} ", DateTime.Now.ToString("yyyy-mm-dd HH:mm:ss"));
			}

			if (args.Any()) sb.AppendFormat(format, args);
			else sb.Append(format);

			sb.AppendLine();

			var buf = Encoding.GetBytes(sb.ToString());
			stream.Write(buf, 0, buf.Length);
			stream.Flush();
		}

		#region Implementation of IDisposable

		/// <summary>
		///     Performs application-defined tasks associated with freeing, releasing, or resetting unmanaged resources.
		/// </summary>
		public void Dispose()
		{
			Dispose(true);
			GC.SuppressFinalize(this);
		}

		protected virtual void Dispose(bool disposing)
		{
			if (disposing) stream.Close();
		}

		#endregion

		#region Implementation of ILog

		public Encoding Encoding { get; set; }

		public LogThreshold Threshold { get; set; }

		public bool PrefixLog { get; set; }

		public string Prefix { get; set; }

		public void Debug(string message)
		{
			Write(LogThreshold.Debug, message);
		}

		public void DebugFormat(string format, params object[] args)
		{
			Write(LogThreshold.Debug, format, args);
		}

		public void Error(string message)
		{
			Write(LogThreshold.Error, message);
		}

		public void ErrorFormat(string format, params object[] args)
		{
			Write(LogThreshold.Error, format, args);
		}

		public void Fatal(string message)
		{
			Write(LogThreshold.Fatal, message);
		}

		public void FatalFormat(string format, params object[] args)
		{
			Write(LogThreshold.Fatal, format, args);
		}

		public void Info(string message)
		{
			Write(LogThreshold.Info, message);
		}

		public void InfoFormat(string format, params object[] args)
		{
			Write(LogThreshold.Info, format, args);
		}

		public void Warn(string message)
		{
			Write(LogThreshold.Warning, message);
		}

		public void WarnFormat(string format, params object[] args)
		{
			Write(LogThreshold.Warning, format, args);
		}

		#endregion
	}
}
