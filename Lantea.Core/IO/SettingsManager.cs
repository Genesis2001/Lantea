// -----------------------------------------------------------------------------
//  <copyright file="SettingsManager.cs" company="Zack Loveless">
//      Copyright (c) Zack Loveless.  All rights reserved.
//  </copyright>
// -----------------------------------------------------------------------------
namespace Lantea.Core.IO
{
	using System.IO;
	using System.Xml.Linq;
	using Common.IO;

	public class SettingsManager : ISettingsManager
	{
		private readonly string configFile;
		private FileStream stream;
		private XDocument document;

		public SettingsManager(string configFile)
		{
			this.configFile = configFile;
		}

		#region Implementation of ISettingsManager

		public string GetValue(string key)
		{
			throw new System.NotImplementedException();
		}

		public void Load()
		{
			stream   = new FileStream(configFile, FileMode.Open, FileAccess.Read, FileShare.Read);
			document = XDocument.Load(stream);
		}

		#endregion
	}
}
