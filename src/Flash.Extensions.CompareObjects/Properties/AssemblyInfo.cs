using System;
using System.Reflection;
using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;

[assembly: AssemblyTitle("Flash")]
[assembly: AssemblyDescription("Core classes of Flash that are independent of any framework.")]
[assembly: Guid("F5F84CAF-B1F2-43CE-92FC-49BA342A1262")]
[assembly: CLSCompliant(true)]
[assembly: InternalsVisibleTo("Flash.Extensions.CompareObjects")]

// Allow the generation of mocks for internal types
[assembly: InternalsVisibleTo("DynamicProxyGenAssembly2")]