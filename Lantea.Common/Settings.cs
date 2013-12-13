// -----------------------------------------------------------------------------
//  <copyright file="Settings.cs" company="Zack Loveless">
//      Copyright (c) Zack Loveless.  All rights reserved.
//      
//      LICENSE TBA
//  </copyright>
// -----------------------------------------------------------------------------

namespace Lantea.Common
{
	using System.IO;

	public interface IConfigDocument
	{
		string Read(string path);
	}

	public class Settings
	{
		private IConfigDocument document;

		private Settings(Stream inputStream)
		{
			// 
		}

		public bool IsLoaded { get; private set; }

		private void Load()
		{
			IsLoaded = true;
		}

		public string GetValue(string path)
		{
			return document.Read(path);
		}

		#region Factory Initializers

		public static Settings LoadFrom(string path)
		{
			// var stream = new FileStream(path, FileMode.Open, FileAccess.Read, FileShare.ReadWrite);

			var settings = new Settings(null);
			settings.Load();

			return settings;
		}

		public static Settings LoadFrom(Stream inputStream)
		{
			var settings = new Settings(inputStream);
			settings.Load();

			return settings;
		}

		public static bool Save(string path, Settings settings)
		{
			return true;
		}

		#endregion
	}
}
