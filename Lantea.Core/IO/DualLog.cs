// -----------------------------------------------------------------------------
//  <copyright file="DualLog.cs" company="Zack Loveless">
//      Copyright (c) Zack Loveless.  All rights reserved.
//  </copyright>
// -----------------------------------------------------------------------------

namespace Lantea.Core.IO
{
	using System;

	public class DualLog : ILog
	{
		private readonly ILog log1;
		private readonly ILog log2;

		public DualLog(ILog log1, ILog log2)
		{
			this.log1 = log1;
			this.log2 = log2;
		}

		#region Implementation of IDisposable

		/// <summary>
		/// Performs application-defined tasks associated with freeing, releasing, or resetting unmanaged resources.
		/// </summary>
		public void Dispose()
		{
			Dispose(true);
			GC.SuppressFinalize(this);
		}

		protected virtual void Dispose(bool disposing)
		{
			if (!disposing) return;

			log1.Dispose();
			log2.Dispose();
		}

		#endregion

		#region Implementation of ILog

		public LogThreshold Threshold
		{
			get { throw new NotImplementedException(); }
			set
			{
				log1.Threshold = value;
				log2.Threshold = value;
			}
		}

		public bool PrefixLog
		{
			get { return log1.PrefixLog && log2.PrefixLog; }
			set
			{
				log1.PrefixLog = value;
				log2.PrefixLog = value;
			}
		}

		public string Prefix
		{
			get { throw new NotImplementedException(); }
			set
			{
				log1.Prefix = value;
				log2.Prefix = value;
			}
		}

		public void Debug(string line)
		{
			log1.Debug(line);
			log2.Debug(line);
		}

		public void DebugFormat(string format, params object[] args)
		{
			log1.DebugFormat(format, args);
			log2.DebugFormat(format, args);
		}

		public void Error(string line)
		{
			log1.Error(line);
			log2.Error(line);
		}

		public void ErrorFormat(string format, params object[] args)
		{
			log1.ErrorFormat(format, args);
			log2.ErrorFormat(format, args);
		}

		public void Fatal(string line)
		{
			log1.Fatal(line);
			log2.Fatal(line);
		}

		public void FatalFormat(string format, params object[] args)
		{
			log1.FatalFormat(format, args);
			log2.FatalFormat(format, args);
		}

		public void Info(string line)
		{
			log1.Info(line);
			log2.Info(line);
		}

		public void InfoFormat(string format, params object[] args)
		{
			log1.InfoFormat(format, args);
			log2.InfoFormat(format, args);
		}

		public void Warn(string line)
		{
			log1.Warn(line);
			log2.Warn(line);
		}

		public void WarnFormat(string format, params object[] args)
		{
			log1.WarnFormat(format, args);
			log2.WarnFormat(format, args);
		}

		#endregion
	}
}
