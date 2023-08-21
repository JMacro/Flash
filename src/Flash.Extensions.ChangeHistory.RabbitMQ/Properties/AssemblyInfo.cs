using System;
using System.Reflection;
using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;

[assembly: AssemblyTitle("Flash")]
[assembly: AssemblyDescription("Core classes of Flash that are independent of any framework.")]
[assembly: Guid("1A197A08-BFC0-4F30-8B33-01C4CA6A0A53")]
[assembly: CLSCompliant(true)]
[assembly: InternalsVisibleTo("Flash.Extensions.ChangeHistory.RabbitMQ")]

// Allow the generation of mocks for internal types
[assembly: InternalsVisibleTo("DynamicProxyGenAssembly2")]