// -----------------------------------------------------------------------------
//  <copyright file="ISettingsManager.cs" company="Zack Loveless">
//      Copyright (c) Zack Loveless.  All rights reserved.
//  </copyright>
// -----------------------------------------------------------------------------

namespace Lantea.Core.IO
{
	public interface ISettingsManager
	{
		string GetValue(string key);

		string[] GetValues(string key);

		void Load();
	}
}
