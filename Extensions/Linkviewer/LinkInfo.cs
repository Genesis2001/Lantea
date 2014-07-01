// -----------------------------------------------------------------------------
//  <copyright file="LinkInfo.cs" company="Zack Loveless">
//      Copyright (c) Zack Loveless.  All rights reserved.
//  </copyright>
// -----------------------------------------------------------------------------

namespace Linkviewer
{
    using System.Collections.Generic;

    public class LinkInfo
    {
        public LinkInfo()
        {
            Meta = new Dictionary<string, string>();
        }

        public string Title { get; set; }

        public string Uri { get; set; }

        public Dictionary<string, string> Meta { get; private set; }
    }
}
