// -----------------------------------------------------------------------------
//  <copyright file="Log.cs" company="Zack Loveless">
//      Copyright (c) Zack Loveless.  All rights reserved.
//  </copyright>
// -----------------------------------------------------------------------------

namespace Lantea.Core.IO
{
	using System;
	using System.IO;
	using System.Text;
	using Atlantis.IO;

	public class Log : ILog
	{
		private readonly string fileName;
		private readonly FileStream stream;

		public Log(string fileName)
		{
			this.fileName = fileName;

			stream = new FileStream(fileName, FileMode.Append, FileAccess.Write, FileShare.Read);
		}

		private void Write(LogThreshold threshold, string format, params object[] args)
		{
			if (!Threshold.HasFlag(threshold)) return;

			var sb = new StringBuilder();

			if (PrefixLog)
			{
				if (!string.IsNullOrEmpty(Prefix)) sb.Append(Prefix);
				else sb.AppendFormat(" {0} ", DateTime.Now.ToString("yy-mm-dd HH:mm:ss"));
			}

			sb.AppendFormat(format, args);
			sb.AppendLine();

			var buf = Encoding.GetBytes(sb.ToString());
			stream.Write(buf, 0, buf.Length);
			stream.Flush();
		}

		#region Implementation of ILog

		public Encoding Encoding { get; set; }

		public LogThreshold Threshold { get; set; }

		public bool PrefixLog { get; set; }

		public string Prefix { get; set; }

		public void Debug(string message)
		{
			if (Threshold.HasFlag(LogThreshold.Debug)) Write(LogThreshold.Debug, message);
		}

		public void DebugFormat(string format, params object[] args)
		{
			if (Threshold.HasFlag(LogThreshold.Debug)) Write(LogThreshold.Debug, format, args);
		}

		public void Error(string message)
		{
			if (Threshold.HasFlag(LogThreshold.Error)) Write(LogThreshold.Error, message);
		}

		public void ErrorFormat(string format, params object[] args)
		{
			if (Threshold.HasFlag(LogThreshold.Error)) Write(LogThreshold.Error, format, args);
		}

		public void Fatal(string message)
		{
			if (Threshold.HasFlag(LogThreshold.Error)) Write(LogThreshold.Error, message);
		}

		public void FatalFormat(string format, params object[] args)
		{
			if (Threshold.HasFlag(LogThreshold.Error)) Write(LogThreshold.Error, format, args);
		}

		public void Info(string message)
		{
			if ((Threshold & LogThreshold.Info) == LogThreshold.Info) Write(LogThreshold.Info, message);
		}

		public void InfoFormat(string format, params object[] args)
		{
			if (Threshold.HasFlag(LogThreshold.Info)) Write(LogThreshold.Info, format, args);
		}

		public void Warn(string message)
		{
			if (Threshold.HasFlag(LogThreshold.Warning)) Write(LogThreshold.Warning, message);
		}

		public void WarnFormat(string format, params object[] args)
		{
			if (Threshold.HasFlag(LogThreshold.Warning)) Write(LogThreshold.Warning, format, args);
		}

		#endregion

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
			if (!disposing) return;

			stream.Close();
		}

		#endregion
	}
}
