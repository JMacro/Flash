Properties {
    ### Directories
    $base_dir = resolve-path .
    $build_dir = "$base_dir\build"
    $src_dir = "$base_dir\src"
    $tests_dir = "$base_dir\tests"
    $package_dir = "$base_dir\packages"
    $nuspec_dir = "$base_dir\nuspecs"
    $nupkg_dir = "$build_dir\nupkgs"
    $temp_dir = "$build_dir\temp"
    $framework_dir =  $env:windir + "\Microsoft.Net\Framework\v4.0.30319"

    ### Tools
    $nuget = "$base_dir\.nuget\nuget.exe"
    $ilmerge = "$package_dir\ilmerge.*\tools\ilmerge.exe"
    $xunit = "$package_dir\xunit.runners*\tools\xunit.console.clr4.exe"
    $7zip = "$package_dir\7-Zip.CommandLine.*\tools\7za.exe"
    $opencover = "$package_dir\OpenCover.*\opencover.console.exe"

    ### AppVeyor-related
    $appVeyorConfig = "$base_dir\appveyor.yml"
    $appVeyor = $env:APPVEYOR

    ### Project information
    $solution_path = "$base_dir\$solution"
    $config = "Release"    
    $sharedAssemblyInfo = "$src_dir\SharedAssemblyInfo.cs"
    $config_Nupkg = "$base_dir\Push-Nupkg.json"
}

## Tasks

Task Restore -Description "Restore NuGet packages for solution." {
    Write-Host "Restoring NuGet packages for '$solution'..." -ForegroundColor "Green"
    Exec { .$nuget restore $solution }
}

Task Clean -Description "Clean up build and project folders." {
    Clean-Directory $build_dir

    if ($solution) {
        Write-Host "Cleaning up '$solution'..." -ForegroundColor "Green"
        Exec { msbuild $solution_path /target:Clean /nologo /verbosity:minimal }
    }
}

Task Compile -Depends Clean, Restore -Description "Compile all the projects in a solution." {
    Write-Host "Compiling '$solution'..." -ForegroundColor "Green"

    $extra = $null
    if ($appVeyor) {
        $extra = "/logger:C:\Program Files\AppVeyor\BuildAgent\Appveyor.MSBuildLogger.dll"
    }

    Exec { msbuild $solution_path /p:Configuration=$config /nologo /verbosity:minimal $extra }
}

Task Version -Description "Patch AssemblyInfo and AppVeyor version files." {
    $newVersion = Read-Host "Please enter a new version number (major.minor.patch)"
    Update-SharedVersion $newVersion
    Update-AppVeyorVersion $newVersion
}

Task Push-Nupkg -Description "Push NuGet packages." {
    $version = Get-PackageVersion
    $nugetApiKeys = Read-Host "Please enter a nuget api keys"
    if(!$nugetApiKeys) {
        Write-Host "Nuget api keys is null,Please try enter a nuget api keys." -ForegroundColor "Red"
        return
    }
    Write-Host "Read Config" -ForegroundColor "Green"
    $config = Get-PushNupkgContent -path $config_Nupkg
    $nupkgs = $config.Nupkgs

    Write-Host "Nuget package version '$version'" -ForegroundColor "Red"

    Write-Host "Nuget package path in '$nupkg_dir'..." -ForegroundColor "Green"
    $dirs = Get-ChildItem -Path "$nupkg_dir\*" -Filter "*.nupkg" -Exclude "*.symbols.nupkg"

    $cur_time = Get-Date -Format "yyyy-MM-dd HH:mm:ss K"
    $push_list = @()
    $push_list += "------------------------$cur_time ($version)------------------------"
    Try {
        foreach ($dir in $dirs) {
            $packName = $dir.BaseName -replace ".$version",""
            $item = $nupkgs | where-Object 'PackName' $packName -EQ 
            if (!$item.IsPush){
                Write-Host "Ignore pack name $packName"
                continue
            }

            Write-Host "NuGet push '$dir'..." -ForegroundColor "Green"
            Try {
                Exec { .$nuget push $dir -source https://api.nuget.org/v3/index.json -ApiKey $nugetApiKeys }

                $push_list += $packName
            }
            Catch { }
        }
    }
    Catch { }
    $push_list += "-------------------------------------------------------------------------------------"
    $push_list += [Environment]::NewLine

    Write-Host "Nuget package push details " -ForegroundColor "Green"
    foreach ($row in $push_list) {
        Write-Host "'$row'" -ForegroundColor "Green"
    }

    $push_str = $push_list -join [Environment]::NewLine
    $push_str | Add-Content "$base_dir\PushDetails.Nuget"

}

## Functions

### Test functions

function Run-XunitTests($project, $target) {
    Write-Host "Running xUnit test runner for '$project'..." -ForegroundColor "Green"
    $assembly = (Get-TestsOutputDir $project $target) + "\$project.dll"

    if ($appVeyor) {
        Exec { xunit.console.clr4 $assembly /appveyor }
    } else {
        Exec { .$xunit $assembly }
    }
}

function Run-OpenCover($projectWithOptionalTarget, $coverageFile, $coverageFilter) {
    $project = $projectWithOptionalTarget
    $target = $null

    if ($projectWithOptionalTarget -Is [System.Array]) {
        $project = $projectWithOptionalTarget[0]
        $target = $projectWithOptionalTarget[1]
    }

    if ($env:APPVEYOR) {
        $xunit_path = Get-Command "xunit.console.clr4.exe" | Select-Object -ExpandProperty Definition
        $extra = "/appveyor"
    }
    else {
        # We need to use paths without asterisks here
        $xunit_path = Resolve-Path $xunit
    }

    Write-Host "Running OpenCover/xUnit for '$project'..." -ForegroundColor "Green"
    $assembly = (Get-TestsOutputDir $project $target) + "\$project.dll"

    Exec {        
        .$opencover `"-target:$xunit_path`" `"-targetargs:$assembly /noshadow $extra`" `"-filter:$coverageFilter`" -mergeoutput `"-output:$coverageFile`" -register:user -returntargetcode
    }
}

### Merge functions

function Merge-Assembly($projectWithOptionalTarget, $internalizeAssemblies, $target) {
    $project = $projectWithOptionalTarget
    $target = $null

    if ($projectWithOptionalTarget -Is [System.Array]) {
        $project = $projectWithOptionalTarget[0]
        $target = $projectWithOptionalTarget[1]
    }

    Write-Host "Merging '$project' with $internalizeAssemblies..." -ForegroundColor "Green"

    $internalizePaths = @()

    $projectOutput = Get-SrcOutputDir $project $target

    foreach ($assembly in $internalizeAssemblies) {
        $internalizePaths += "$projectOutput\$assembly.dll"
    }

    $primaryAssemblyPath = "$projectOutput\$project.dll"

    Create-Directory $temp_dir
    
    Exec { .$ilmerge /targetplatform:"v4,$framework_dir" `
        /out:"$temp_dir\$project.dll" `
        /target:library `
        /internalize `
        $primaryAssemblyPath `
        $internalizePaths `
    }

    Move-Files "$temp_dir\$project.*" $projectOutput
}

### Collect functions

function Collect-Tool($source) {
    Write-Host "Collecting tool '$source'..." -ForegroundColor "Green"

    $destination = "$build_dir\tools"

    Create-Directory $destination
    Copy-Files "$source" $destination
}

function Collect-Content($source) {
    Write-Host "Collecting content '$source'..." -ForegroundColor "Green"

    $destination = "$build_dir\content"

    Create-Directory $destination
    Copy-Files "$source" $destination
}

function Collect-Assembly($project, $target) {
    Write-Host "Collecting assembly '$target/$project'..." -ForegroundColor "Green"
    
    $source = (Get-SrcOutputDir $project $target) + "\$project.*"
    $destination = "$build_dir\$target"

    Create-Directory $destination
    Copy-Files $source $destination
}

function Collect-File($source) {
    Write-Host "Collecting file '$source'..." -ForegroundColor "Green"

    $destination = $build_dir;

    Create-Directory $destination
    Copy-Files $source $destination
}

### Pack functions

function Create-Package($project, $version) {
    Write-Host "Creating NuGet package for '$project'..." -ForegroundColor "Green"

    Create-Directory $temp_dir
    Copy-Files "$nuspec_dir\$project.nuspec" $temp_dir

    Try {
        Replace-Content "$nuspec_dir\$project.nuspec" '0.0.0' $version
        Exec { .$nuget pack "$nuspec_dir\$project.nuspec" -OutputDirectory "$nupkg_dir" -BasePath "$build_dir" -Version "$version" -Symbols }
    }
    Finally {
        Move-Files "$temp_dir\$project.nuspec" $nuspec_dir
    }
}

### Version functions

function Get-PackageVersion {
    $version = Get-BuildVersion

	$tag = $env:APPVEYOR_REPO_TAG_NAME
    if ($tag -And $tag.StartsWith("v$version-")) {
        $version = $tag.Substring(1)
        Write-Host "Using tag-based version '$version' for packages..." -ForegroundColor "Green"
    }

    return $version
}

function Get-BuildVersion {
    $version = Get-SharedVersion
    $buildNumber = $env:APPVEYOR_BUILD_NUMBER

    if ($env:APPVEYOR_REPO_TAG -ne "True" -And $buildNumber -ne $null) {
        $version += "-build-" + $buildNumber.ToString().PadLeft(5, '0')
        Write-Host "Using CI build version '$version'..." -ForegroundColor "Green"
    }

    return $version
}

function Get-SharedVersion {
    $line = Get-Content "$sharedAssemblyInfo" | where {$_.Contains("AssemblyVersion")}
    $line.Split('"')[1]
}

function Update-SharedVersion($version) {
    Check-Version($version)
        
    $versionPattern = 'AssemblyVersion\("[0-9]+(\.([0-9]+|\*)){1,3}"\)'
    $versionAssembly = 'AssemblyVersion("' + $version + '")';

    if (Test-Path $sharedAssemblyInfo) {
        Write-Host "Patching '$sharedAssemblyInfo'..." -ForegroundColor "Green"
        Replace-Content "$sharedAssemblyInfo" $versionPattern $versionAssembly
    }
}

function Update-AppveyorVersion($version) {
    Check-Version($version)

    $versionPattern = "version: [0-9]+(\.([0-9]+|\*)){1,3}"
    $versionReplace = "version: $version"

    if (Test-Path $appVeyorConfig) {
        Write-Host "Patching '$appVeyorConfig'..." -ForegroundColor "Green"
        Replace-Content "$appVeyorConfig" $versionPattern $versionReplace
    }
}

function Check-Version($version) {
    if ($version -notmatch "[0-9]+(\.([0-9]+|\*)){1,3}") {
        Write-Error "Version number incorrect format: $version"
    }
}

### Archive functions

function Create-Archive($name) {
    Write-Host "Creating archive '$name.zip'..." -ForegroundColor "Green"
    Remove-Directory $temp_dir
    Create-Zip "$build_dir\$name.zip" "$build_dir"
}

function Create-Zip($file, $dir){
    if (Test-Path -path $file) { Remove-Item $file }
    Create-Directory $dir
    Exec { & $7zip a -mx -tzip $file $dir\* } 
}

### Common functions

function Create-Directory($dir) {
    New-Item -Path $dir -Type Directory -Force > $null
}

function Clean-Directory($dir) {
    If (Test-Path $dir) {
        Write-Host "Cleaning up '$dir'..." -ForegroundColor "DarkGray"
        Remove-Item "$dir\*" -Recurse -Force
    }
}

function Remove-File($file) {
    if (Test-Path $file) {
        Write-Host "Removing '$file'..." -ForegroundColor "DarkGray"
        Remove-Item $file -Force
    }
}

function Remove-Directory($dir) {
    if (Test-Path $dir) {
        Write-Host "Removing '$dir'..." -ForegroundColor "DarkGray"
        Remove-Item $dir -Recurse -Force
    }
}

function Copy-Files($source, $destination) {
    Copy-Item "$source" $destination -Force > $null
}

function Move-Files($source, $destination) {
    Move-Item "$source" $destination -Force > $null
}

function Replace-Content($file, $pattern, $substring) {
    (gc $file) -Replace $pattern, $substring | sc $file
}

function Get-SrcOutputDir($project, $target) {
    $result = _Get-OutputDir $src_dir $project $target

    Write-Host "  Using directory $result" -ForegroundColor "DarkGray"
    return $result
}

function Get-TestsOutputDir($project, $target) {
    $result = _Get-OutputDir $tests_dir $project $target

    Write-Host "  Using directory $result" -ForegroundColor "DarkGray"
    return $result
}

function _Get-OutputDir($dir, $project, $target) {
    $baseDir = "$dir\$project\bin"
    
    if ($target -And (Test-Path "$baseDir\$target\$config")) {
        return "$baseDir\$target\$config"
    }

    if ($target -And (Test-Path "$baseDir\$config\$target")) {
        return "$baseDir\$config\$target"
    }

    return "$baseDir\$config"
}

function Get-PushNupkgContent($path){
    $json = Get-Content -Raw -Encoding "UTF8" -Path "$path"
    return ConvertFrom-Json -InputObject $json
}