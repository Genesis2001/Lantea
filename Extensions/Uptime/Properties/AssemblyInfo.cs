﻿using System;
using System.Reflection;

#if DEBUG
[assembly: AssemblyConfiguration("Debug")]
#else
[assembly: AssemblyConfiguration("Release")]
#endif

[assembly: AssemblyTitle("Check")]
[assembly: AssemblyDescription("A module to retrieve the uptime of a service or server.")]
[assembly: AssemblyCompany("Zack Loveless")]
[assembly: AssemblyProduct("Check Module")]
[assembly: AssemblyCopyright("Copyright © Zack Loveless. All Rights Reserved.")]
[assembly: AssemblyTrademark("")]
[assembly: AssemblyCulture("")]
[assembly: CLSCompliant(true)]
[assembly: AssemblyVersion("1.0")]
