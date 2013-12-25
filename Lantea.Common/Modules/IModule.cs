﻿// -----------------------------------------------------------------------------
//  <copyright file="IModule.cs" company="Zack Loveless">
//      Copyright (c) Zack Loveless.  All rights reserved.
//  </copyright>
// -----------------------------------------------------------------------------

namespace Lantea.Common.Modules
{
	public interface IModule : IModuleMeta
	{
		void Load();

		void Unload();
	}
}
