﻿//------------------------------------------------------------------------------
// <auto-generated>
//     This code was generated by a tool.
//     Runtime Version:4.0.30319.18444
//
//     Changes to this file may cause incorrect behavior and will be lost if
//     the code is regenerated.
// </auto-generated>
//------------------------------------------------------------------------------

namespace Lantea.UnitTests {
    using System;
    
    
    /// <summary>
    ///   A strongly-typed resource class, for looking up localized strings, etc.
    /// </summary>
    // This class was auto-generated by the StronglyTypedResourceBuilder
    // class via a tool like ResGen or Visual Studio.
    // To add or remove a member, edit your .ResX file then rerun ResGen
    // with the /str option, or rebuild your VS project.
    [global::System.CodeDom.Compiler.GeneratedCodeAttribute("System.Resources.Tools.StronglyTypedResourceBuilder", "4.0.0.0")]
    [global::System.Diagnostics.DebuggerNonUserCodeAttribute()]
    [global::System.Runtime.CompilerServices.CompilerGeneratedAttribute()]
    internal class ConfigurationStrings {
        
        private static global::System.Resources.ResourceManager resourceMan;
        
        private static global::System.Globalization.CultureInfo resourceCulture;
        
        [global::System.Diagnostics.CodeAnalysis.SuppressMessageAttribute("Microsoft.Performance", "CA1811:AvoidUncalledPrivateCode")]
        internal ConfigurationStrings() {
        }
        
        /// <summary>
        ///   Returns the cached ResourceManager instance used by this class.
        /// </summary>
        [global::System.ComponentModel.EditorBrowsableAttribute(global::System.ComponentModel.EditorBrowsableState.Advanced)]
        internal static global::System.Resources.ResourceManager ResourceManager {
            get {
                if (object.ReferenceEquals(resourceMan, null)) {
                    global::System.Resources.ResourceManager temp = new global::System.Resources.ResourceManager("Lantea.UnitTests.ConfigurationStrings", typeof(ConfigurationStrings).Assembly);
                    resourceMan = temp;
                }
                return resourceMan;
            }
        }
        
        /// <summary>
        ///   Overrides the current thread's CurrentUICulture property for all
        ///   resource lookups using this strongly typed resource class.
        /// </summary>
        [global::System.ComponentModel.EditorBrowsableAttribute(global::System.ComponentModel.EditorBrowsableState.Advanced)]
        internal static global::System.Globalization.CultureInfo Culture {
            get {
                return resourceCulture;
            }
            set {
                resourceCulture = value;
            }
        }
        
        /// <summary>
        ///   Looks up a localized string similar to .
        /// </summary>
        internal static string EmptyFile {
            get {
                return ResourceManager.GetString("EmptyFile", resourceCulture);
            }
        }
        
        /// <summary>
        ///   Looks up a localized string similar to block
        ///{
        ///	name = &quot;some block&quot;
        ///}.
        /// </summary>
        internal static string SingleBlockWithNameProperty {
            get {
                return ResourceManager.GetString("SingleBlockWithNameProperty", resourceCulture);
            }
        }
        
        /// <summary>
        ///   Looks up a localized string similar to block
        ///{
        ///}.
        /// </summary>
        internal static string SingleEmptyBlock {
            get {
                return ResourceManager.GetString("SingleEmptyBlock", resourceCulture);
            }
        }
        
        /// <summary>
        ///   Looks up a localized string similar to block
        ///{
        ///}
        ///
        ///block
        ///{
        ///}
        ///
        ///block
        ///{
        ///}.
        /// </summary>
        internal static string ThreeEmptyBlocks {
            get {
                return ResourceManager.GetString("ThreeEmptyBlocks", resourceCulture);
            }
        }
    }
}
