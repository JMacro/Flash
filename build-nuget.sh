/bin/bash
echo "Building..."
workDir=$(pwd)
dotnet pack ${workDir}/Flash.Core --output ${workDir}/nupkgs
dotnet pack ${workDir}/Flash.Extersions.Cache --output ${workDir}/nupkgs
dotnet pack ${workDir}/Flash.Extersions.Cache.Redis --output ${workDir}/nupkgs
dotnet pack ${workDir}/Flash.Extersions.CQRS --output ${workDir}/nupkgs
dotnet pack ${workDir}/Flash.Extersions.Middlewares --output ${workDir}/nupkgs
dotnet pack ${workDir}/Flash.Extersions.OpenTracting --output ${workDir}/nupkgs
dotnet pack ${workDir}/Flash.Extersions.RabbitMQ --output ${workDir}/nupkgs
dotnet pack ${workDir}/Flash.Extersions.Security --output ${workDir}/nupkgs
dotnet pack ${workDir}/Flash.Extersions.System --output ${workDir}/nupkgs
dotnet pack ${workDir}/Flash.Extersions.UidGenerator --output ${workDir}/nupkgs
dotnet pack ${workDir}/Flash.LoadBalancer --output ${workDir}/nupkgs
echo "End"