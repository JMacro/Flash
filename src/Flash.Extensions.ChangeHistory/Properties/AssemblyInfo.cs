using System;
using System.Reflection;
using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;

[assembly: AssemblyTitle("Flash")]
[assembly: AssemblyDescription("Core classes of Flash that are independent of any framework.")]
[assembly: Guid("6C0039B3-2A07-4DEB-A89D-92436E253536")]
[assembly: CLSCompliant(true)]
[assembly: InternalsVisibleTo("Flash.Extensions.ChangeHistory")]

// Allow the generation of mocks for internal types
[assembly: InternalsVisibleTo("DynamicProxyGenAssembly2")]