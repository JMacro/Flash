set workDir=%cd%
set nupkgs_path=%workDir%\nupkgs
echo "Building..."
for /d %%i in (Flash.*) do (
	dotnet pack %workDir%\%%i --output %nupkgs_path%	
)
echo "Build End"
start explorer %nupkgs_path%
pause