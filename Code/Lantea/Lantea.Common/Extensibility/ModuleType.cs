// -----------------------------------------------------------------------------
//  <copyright file="ModuleType.cs" company="Zack Loveless">
//      Copyright (c) Zack Loveless.  All rights reserved.
//  </copyright>
// -----------------------------------------------------------------------------

namespace Lantea.Common.Extensibility
{
    public enum ModuleType
    {
        /// <summary>
        ///     Indicates the module is maintained by a 3rd party.
        /// </summary>
        THIRD = 0,

        /// <summary>
        ///     Indicates the module is maintaind by the vendor.
        /// </summary>
        VENDOR = 1,

        /// <summary>
        ///     Indicates the module is maintaind by a member of the development team separately from the project.
        /// </summary>
        EXTRA = 2,
    }
}
