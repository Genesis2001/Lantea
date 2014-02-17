// -----------------------------------------------------------------------------
//  <copyright file="ConfigDocument.cs" company="Zack Loveless">
//      Copyright (c) Zack Loveless.  All rights reserved.
//  </copyright>
// -----------------------------------------------------------------------------

namespace Lantea.Core.IO
{
	using System.Collections.Generic;

	public class ConfigDocument
	{
		private readonly HashSet<ConfigReader.Block> blocks;

		internal ConfigDocument(IEnumerable<ConfigReader.Block> blocks)
		{
			this.blocks = new HashSet<ConfigReader.Block>(blocks);
		}
	}
}
