// -----------------------------------------------------------------------------
//  <copyright file="SettingsManager.cs" company="Zack Loveless">
//      Copyright (c) Zack Loveless.  All rights reserved.
//  </copyright>
// -----------------------------------------------------------------------------

namespace Lantea.Core.IO
{
	using System;
	using System.IO;
	using System.Xml.XPath;

	public class SettingsManager : ISettingsManager
	{
		private readonly string configFile;
		private FileStream stream;
		private XPathDocument document;
		private XPathNavigator navigator;

		public SettingsManager(string configFile)
		{
			this.configFile = configFile;
		}

		#region Implementation of ISettingsManager

		public string GetValue(string key)
		{
			var expr = navigator.Compile(key);
			var node = navigator.SelectSingleNode(expr);

			return node != null ? node.Value : string.Empty;
		}

		public string[] GetValues(string key)
		{
			throw new NotImplementedException();
		}

		public void Load()
		{
			stream    = new FileStream(configFile, FileMode.Open, FileAccess.Read, FileShare.Read);
			document  = new XPathDocument(stream);
			navigator = document.CreateNavigator();
		}

		#endregion
	}
}
