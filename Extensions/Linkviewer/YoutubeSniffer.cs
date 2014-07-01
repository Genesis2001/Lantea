// -----------------------------------------------------------------------------
//  <copyright file="YoutubeSniffer.cs" company="Zack Loveless">
//      Copyright (c) Zack Loveless.  All rights reserved.
//  </copyright>
// -----------------------------------------------------------------------------

namespace Linkviewer
{
    public class YoutubeSniffer : ILinkSniffer
    {
        #region Implementation of ILinkSniffer

        public string Name
        {
            get { return "YouTube"; }
        }

        public string Regex
        {
            get { return @"(?:http:\/\/?)(?:www\.?)youtu(?:\.be\/|be\.com\/watch\?v=(.+)"; }
        }

        public LinkInfo Sniff(string url)
        {
            return null;
        }

        #endregion
    }
}
