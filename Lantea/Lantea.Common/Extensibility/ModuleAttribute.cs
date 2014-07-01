// -----------------------------------------------------------------------------
//  <copyright file="ModuleAttribute.cs" company="Zack Loveless">
//      Copyright (c) Zack Loveless.  All rights reserved.
//  </copyright>
// -----------------------------------------------------------------------------

namespace Lantea.Common.Extensibility
{
    using System;
    using System.ComponentModel.Composition;

    [AttributeUsage(AttributeTargets.Class, AllowMultiple = false), MetadataAttribute]
    public class ModuleAttribute : ExportAttribute, IModuleAttribute
    {
        public ModuleAttribute() : base(typeof(IModule))
        {
        }

        #region Implementation of IModuleAttribute

        /// <summary>
        ///     <para>Gets or sets a <see cref="T:System.Boolean" /> value indicating whether the bot will instantiate a new IrcClient for the module.</para>
        ///     <para>If not set, will reuse the same IrcClient that the bot instantiates for itself. Defaults to false.</para>
        /// </summary>
        public bool CreateNewClient { get; set; }

        /// <summary>
        ///     <para>Gets or sets a <see cref="T:System.String" /> value representing the name of the configuration block for the module.</para>
        ///     <para>If not set, will use the name of the module.</para>
        /// </summary>
        public string ConfigBlock { get; set; }

        #endregion
    }
}
