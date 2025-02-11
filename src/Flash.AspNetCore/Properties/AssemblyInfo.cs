using System;
using System.Reflection;
using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;

[assembly: AssemblyTitle("Flash")]
[assembly: AssemblyDescription("Core classes of Flash that are independent of any framework.")]
[assembly: Guid("3C79BA2D-705A-44D8-83F3-3620A63C206A")]
[assembly: CLSCompliant(true)]
[assembly: InternalsVisibleTo("Flash.Core.Tests")]

// Allow the generation of mocks for internal types
[assembly: InternalsVisibleTo("DynamicProxyGenAssembly2")]