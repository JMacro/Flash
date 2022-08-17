set workDir=%cd%
set nupkgs_path=%workDir%\nupkgs
set packageVersion=11.0.1
echo "Building..."
rmdir /s/q %nupkgs_path%
dotnet pack -p:PackageVersion=%packageVersion% %workDir%\src\Flash.Core --output %nupkgs_path% --nologo
dotnet pack -p:PackageVersion=%packageVersion% %workDir%\src\Flash.DynamicRoute --output %nupkgs_path% --nologo
dotnet pack -p:PackageVersion=%packageVersion% %workDir%\src\Flash.LoadBalancer --output %nupkgs_path% --nologo

for /d %%i in (%workDir%\src\Flash.Extensions.*) do (
	dotnet pack -p:PackageVersion=%packageVersion% %%i --output %nupkgs_path% --nologo
)
echo "Build End"
pause
start explorer %nupkgs_path%