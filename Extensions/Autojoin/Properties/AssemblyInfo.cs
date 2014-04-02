using System;
using System.Reflection;

#if DEBUG
[assembly: AssemblyConfiguration("Debug")]
#else
[assembly: AssemblyConfiguration("Release")]
#endif

[assembly: AssemblyTitle("Autojoin")]
[assembly: AssemblyDescription("An autojoin module for Lantea to join channels upon connecting to the server.")]
[assembly: AssemblyCompany("Zack Loveless")]
[assembly: AssemblyProduct("Autojoin Module")]
[assembly: AssemblyCopyright("Copyright © Zack Loveless. All Rights Reserved.")]
[assembly: AssemblyTrademark("")]
[assembly: AssemblyCulture("")]
[assembly: CLSCompliant(true)]
[assembly: AssemblyVersion("1.0")]
