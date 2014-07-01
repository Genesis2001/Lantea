// -----------------------------------------------------------------------------
//  <copyright file="IModuleAttribute.cs" company="Zack Loveless">
//      Copyright (c) Zack Loveless.  All rights reserved.
//  </copyright>
// -----------------------------------------------------------------------------

namespace Lantea.Common.Extensibility
{
    public interface IModuleAttribute
    {
        /// <summary>
        ///     <para>Gets or sets a <see cref="T:System.Boolean" /> value indicating whether the bot will instantiate a new IrcClient for the module.</para>
        ///     <para>If not set, will reuse the same IrcClient that the bot instantiates for itself. Defaults to false.</para>
        /// </summary>
        bool CreateNewClient { get; }

        /// <summary>
        ///     <para>Gets or sets a <see cref="T:System.String" /> value representing the name of the configuration block for the module.</para>
        ///     <para>If not set, will use the name of the module.</para>
        /// </summary>
        string ConfigBlock { get; }
    }
}
