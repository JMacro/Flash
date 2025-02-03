using System;
using System.Reflection;
using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;

[assembly: AssemblyTitle("Flash")]
[assembly: AssemblyDescription("Core classes of Flash that are independent of any framework.")]
[assembly: Guid("AB043AC4-D279-4FB3-BC7D-92BEABFE0C86")]
[assembly: CLSCompliant(true)]
[assembly: InternalsVisibleTo("Flash.Core.Tests")]

// Allow the generation of mocks for internal types
[assembly: InternalsVisibleTo("DynamicProxyGenAssembly2")]