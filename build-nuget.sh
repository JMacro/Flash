/bin/bash
echo "Building..."
workDir=$(pwd)
dotnet pack ${workDir}/Flash.Core --output ${workDir}/nupkgs
dotnet pack ${workDir}/Flash.Extensions.Cache --output ${workDir}/nupkgs
dotnet pack ${workDir}/Flash.Extensions.Cache.Redis --output ${workDir}/nupkgs
dotnet pack ${workDir}/Flash.Extensions.CQRS --output ${workDir}/nupkgs
dotnet pack ${workDir}/Flash.Extensions.Middlewares --output ${workDir}/nupkgs
dotnet pack ${workDir}/Flash.Extensions.OpenTracting --output ${workDir}/nupkgs
dotnet pack ${workDir}/Flash.Extensions.RabbitMQ --output ${workDir}/nupkgs
dotnet pack ${workDir}/Flash.Extensions.Security --output ${workDir}/nupkgs
dotnet pack ${workDir}/Flash.Extensions.System --output ${workDir}/nupkgs
dotnet pack ${workDir}/Flash.Extensions.UidGenerator --output ${workDir}/nupkgs
dotnet pack ${workDir}/Flash.LoadBalancer --output ${workDir}/nupkgs
echo "End"