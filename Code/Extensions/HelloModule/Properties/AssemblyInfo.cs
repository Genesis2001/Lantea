using System;
using System.Reflection;

#if DEBUG
[assembly: AssemblyConfiguration("Debug")]
#else
[assembly: AssemblyConfiguration("Release")]
#endif

[assembly: AssemblyTitle("Hello")]
[assembly: AssemblyDescription("A hello world module for Lantea introducing the module system.")]
[assembly: AssemblyCompany("Zack Loveless")]
[assembly: AssemblyProduct("Hello Module")]
[assembly: AssemblyCopyright("Copyright © Zack Loveless. All Rights Reserved.")]
[assembly: AssemblyTrademark("")]
[assembly: AssemblyCulture("")]
[assembly: CLSCompliant(true)]
[assembly: AssemblyVersion("1.3")]
