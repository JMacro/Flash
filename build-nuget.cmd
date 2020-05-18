set workDir=%cd%
set nupkgs_path=%workDir%\nupkgs
echo "Building..."
dotnet pack %workDir%\Flash.Core --output %nupkgs_path%
dotnet pack %workDir%\Flash.Extersions.Cache --output %nupkgs_path%
dotnet pack %workDir%\Flash.Extersions.Cache.Redis --output %nupkgs_path%
dotnet pack %workDir%\Flash.Extersions.CQRS --output %nupkgs_path%
dotnet pack %workDir%\Flash.Extersions.Middlewares --output %nupkgs_path%
dotnet pack %workDir%\Flash.Extersions.OpenTracting --output %nupkgs_path%
dotnet pack %workDir%\Flash.Extersions.RabbitMQ --output %nupkgs_path%
dotnet pack %workDir%\Flash.Extersions.Security --output %nupkgs_path%
dotnet pack %workDir%\Flash.Extersions.System --output %nupkgs_path%
dotnet pack %workDir%\Flash.Extersions.UidGenerator --output %nupkgs_path%
dotnet pack %workDir%\Flash.LoadBalancer --output %nupkgs_path%
start explorer %nupkgs_path%
echo "End"
pause