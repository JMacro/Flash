set workDir=%cd%
set PROJECT_PATH=%workDir%\Flash.LoadBalancer
echo %PROJECT_PATH%

dotnet pack %PROJECT_PATH% --output %workDir%\nupkgs
start explorer %workDir%\nupkgs
pause