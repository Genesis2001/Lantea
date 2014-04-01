// -----------------------------------------------------------------------------
//  <copyright file="AutoFileLog.cs" company="Zack Loveless">
//      Copyright (c) Zack Loveless.  All rights reserved.
//  </copyright>
// -----------------------------------------------------------------------------

namespace Lantea.Core
{
	using System;
	using System.IO;
	using System.Text;
	using Atlantis.IO;

	public class AutoFileLog : FileLog
	{
		public AutoFileLog(String path)
			: base(Path.Combine(path, String.Format("lantea.{0}.log", DateTime.Now.ToString("yyyy-MM-dd"))), new UTF8Encoding(false))
		{
		}
	}
}
