// -----------------------------------------------------------------------------
//  <copyright file="ILinkSniffer.cs" company="Zack Loveless">
//      Copyright (c) Zack Loveless.  All rights reserved.
//  </copyright>
// -----------------------------------------------------------------------------

namespace Linkviewer
{
    public interface ILinkSniffer
    {
        string Name { get; }

        string Regex { get; }

        LinkInfo Sniff(string url);
    }
}
