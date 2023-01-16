Framework 4.5.1
Include "psake-common.ps1"

Task Default -Depends Collect
Task CI -Depends Pack

Task Build -Depends Clean -Description "Restore all the packages and build the whole solution." {
    Write-Host "Building ..."
    Exec { dotnet build -c Release }
}

Task Merge -Depends Build -Description "Run ILRepack /internalize to merge required assemblies." {
    #Repack-Assembly("Flash.Extensions.Cache.Redis","net461") @("Polly", "StackExchange.Redis")
    #Repack-Assembly("Flash.Extensions.Cache.Redis","net472") @("Polly", "StackExchange.Redis")
    #Repack-Assembly("Flash.Extensions.Job.Hangfire.AspNetCore", "netcoreapp3.1")
    #Repack-Assembly("Flash.Extensions.Job.Hangfire.AspNetCore", "net5")
    #Repack-Assembly("Flash.Extensions.Job.Hangfire.AspNetCore", "net6")


    # Referenced packages aren't copied to the output folder in .NET Core <= 2.X. To make ILRepack run,
    # we need to copy them using the `dotnet publish` command prior to merging them. In .NET Core 3.0
    # everything should be working without this extra step.
    #Publish-Assembly "Flash.Core" "netstandard2.0"
    #Publish-Assembly "Flash.Core" "netstandard2.1"
    #Publish-Assembly "Flash.Extensions.Cache" "netstandard2.0"
    #Publish-Assembly "Flash.Extensions.Cache.Redis" "netstandard2.0"
    #Publish-Assembly "Flash.Extensions.Cache.Redis.DependencyInjection" "netstandard2.0"
    
    #Publish-Assembly "Flash.Extensions.Job.Hangfire.AspNetCore" "netcoreapp3.1"
    #Publish-Assembly "Flash.Extensions.Job.Hangfire.AspNetCore" "net5"
    #Publish-Assembly "Flash.Extensions.Job.Hangfire.AspNetCore" "net6"

    #Repack-Assembly @("Flash.Extensions.Job.Hangfire.AspNetCore", "netcoreapp3.1")
    #Repack-Assembly @("Flash.Extensions.Job.Hangfire.AspNetCore", "net5")
    #Repack-Assembly @("Flash.Extensions.Job.Hangfire.AspNetCore", "net6")

    #Repack-Assembly @("Flash.Core", "netstandard2.0") @("Castle.Core")
    #Repack-Assembly @("Flash.Core", "netstandard2.1") @("Castle.Core")
    #Repack-Assembly @("Flash.Extensions.Cache", "netstandard2.0") @("Castle.Core")
    #Repack-Assembly @("Flash.Extensions.Cache.Redis", "netstandard2.0") @("Polly", "StackExchange.Redis")
}

Task Test -Depends Merge -Description "Run unit and integration tests against merged assemblies." {
    # Dependencies shouldn't be re-built, because we need to run tests against merged assemblies to test
    # the same assemblies that are distributed to users. Since the `dotnet test` command doesn't support
    # the `--no-dependencies` command directly, we need to re-build tests themselves first.
    Exec { ls "tests\**\*.csproj" | % { dotnet build -c Release --no-dependencies $_.FullName } }

    # We are running unit test project one by one, because pipelined version like the line above does not
    # support halting the whole execution pipeline when "dotnet test" command fails due to a failed test,
    # silently allowing build process to continue its execution even with failed tests.
    # Exec { dotnet test -c Release --no-build "tests\Flash.Test" }
    # Exec { dotnet test -c Release --no-build "tests\Flash.Test.Web" }
}

Task Collect -Depends Test -Description "Copy all artifacts to the build folder." {

    Collect-Assembly "Flash.Extensions.Cache" "netstandard2.0"
    Collect-Assembly "Flash.Extensions.Cache.Redis" "netstandard2.0"
    Collect-Assembly "Flash.Extensions.Cache.Redis.DependencyInjection" "netstandard2.0"
    Collect-Assembly "Flash.Extensions.DistributedLock" "netstandard2.0"

    Collect-Assembly "Flash.Extensions.Configuration.Json" "netstandard2.0"
    Collect-Assembly "Flash.Extensions.Configuration.Apollo" "netstandard2.0"

    Collect-Assembly "Flash.Extensions.EventBus" "netstandard2.0"
    Collect-Assembly "Flash.Extensions.EventBus" "netcoreapp3.1"
    Collect-Assembly "Flash.Extensions.EventBus.RabbitMQ" "netstandard2.0"

    Collect-Assembly "Flash.Extensions.HealthChecks" "netstandard2.0"
    Collect-Assembly "Flash.Extensions.HealthChecks.MySql" "netstandard2.0"
    Collect-Assembly "Flash.Extensions.HealthChecks.RabbitMQ" "netstandard2.0"
    Collect-Assembly "Flash.Extensions.HealthChecks.Redis" "netstandard2.0"

    Collect-Assembly "Flash.Extensions.Office" "netstandard2.0"
    Collect-Assembly "Flash.Extensions.Office.Npoi" "netstandard2.0"

    Collect-Assembly "Flash.Extensions.Job" "netstandard2.0"
    Collect-Assembly "Flash.Extensions.Job.Hangfire" "netstandard2.0"
    Collect-Assembly "Flash.Extensions.Job.Hangfire.AspNetCore" "netstandard2.0"
    Collect-Assembly "Flash.Extensions.Job.Quartz" "netstandard2.0"

    Collect-Assembly "Flash.Extensions.OpenTracting" "netstandard2.0"
    Collect-Assembly "Flash.Extensions.OpenTracting.Jaeger" "netstandard2.0"
    Collect-Assembly "Flash.Extensions.OpenTracting.Jaeger" "netcoreapp3.1"
    Collect-Assembly "Flash.Extensions.OpenTracting.Skywalking" "netstandard2.0"
    Collect-Assembly "Flash.Extensions.OpenTracting.Skywalking" "netcoreapp3.1"

    Collect-Assembly "Flash.Extensions.ORM" "netstandard2.0"
    Collect-Assembly "Flash.Extensions.ORM" "net6.0"
    Collect-Assembly "Flash.Extensions.ORM.EntityFrameworkCore" "netstandard2.0"
    Collect-Assembly "Flash.Extensions.ORM.EntityFrameworkCore" "net6.0"

    Collect-Assembly "Flash.DynamicRoute" "netstandard2.0"
    Collect-Assembly "Flash.Extensions.AppMetrics" "netstandard2.0"    
    Collect-Assembly "Flash.Extensions.Resilience.Http" "netstandard2.0"
    Collect-Assembly "Flash.Extensions.Security" "netstandard2.0"
    Collect-Assembly "Flash.Extensions.UidGenerator" "netstandard2.0"
    Collect-Assembly "Flash.LoadBalancer" "netstandard2.0"
    Collect-Assembly "Flash.Extensions.Email" "netstandard2.0"


    Collect-Content "README.md"

    Collect-Localizations "Flash.Extensions.Cache" "netstandard2.0"
    Collect-Localizations "Flash.Extensions.Cache.Redis" "netstandard2.0"
    Collect-Localizations "Flash.Extensions.Cache.Redis.DependencyInjection" "netstandard2.0"
    Collect-Localizations "Flash.Extensions.DistributedLock" "netstandard2.0"

    Collect-Localizations "Flash.Extensions.Configuration.Json" "netstandard2.0"
    Collect-Localizations "Flash.Extensions.Configuration.Apollo" "netstandard2.0"

    Collect-Localizations "Flash.Extensions.EventBus" "netstandard2.0"
    Collect-Localizations "Flash.Extensions.EventBus" "netcoreapp3.1"
    Collect-Localizations "Flash.Extensions.EventBus.RabbitMQ" "netstandard2.0"

    Collect-Localizations "Flash.Extensions.HealthChecks" "netstandard2.0"
    Collect-Localizations "Flash.Extensions.HealthChecks.MySql" "netstandard2.0"
    Collect-Localizations "Flash.Extensions.HealthChecks.RabbitMQ" "netstandard2.0"
    Collect-Localizations "Flash.Extensions.HealthChecks.Redis" "netstandard2.0"

    Collect-Localizations "Flash.Extensions.Job" "netstandard2.0"
    Collect-Localizations "Flash.Extensions.Job.Hangfire" "netstandard2.0"
    Collect-Localizations "Flash.Extensions.Job.Hangfire.AspNetCore" "netstandard2.0"
    Collect-Localizations "Flash.Extensions.Job.Quartz" "netstandard2.0"

    Collect-Localizations "Flash.Extensions.Office" "netstandard2.0"
    Collect-Localizations "Flash.Extensions.Office.Npoi" "netstandard2.0"

    Collect-Localizations "Flash.Extensions.OpenTracting" "netstandard2.0"
    Collect-Localizations "Flash.Extensions.OpenTracting.Jaeger" "netstandard2.0"
    Collect-Localizations "Flash.Extensions.OpenTracting.Jaeger" "netcoreapp3.1"
    Collect-Localizations "Flash.Extensions.OpenTracting.Skywalking" "netstandard2.0"
    Collect-Localizations "Flash.Extensions.OpenTracting.Skywalking" "netcoreapp3.1"

    Collect-Localizations "Flash.Extensions.ORM" "netstandard2.0"
    Collect-Localizations "Flash.Extensions.ORM" "net6.0"
    Collect-Localizations "Flash.Extensions.ORM.EntityFrameworkCore" "netstandard2.0"
    Collect-Localizations "Flash.Extensions.ORM.EntityFrameworkCore" "net6.0"

    Collect-Localizations "Flash.DynamicRoute" "netstandard2.0"
    Collect-Localizations "Flash.Extensions.AppMetrics" "netstandard2.0"
    Collect-Localizations "Flash.Extensions.Resilience.Http" "netstandard2.0"
    Collect-Localizations "Flash.Extensions.Security" "netstandard2.0"
    Collect-Localizations "Flash.Extensions.UidGenerator" "netstandard2.0"
    Collect-Localizations "Flash.LoadBalancer" "netstandard2.0"
    Collect-Localizations "Flash.Extensions.Email" "netstandard2.0"

    Collect-File "LICENSE.md"
    Collect-File "NOTICES"
    Collect-File "COPYING.LESSER"
    Collect-File "COPYING"
    Collect-File "LICENSE_STANDARD"
    Collect-File "LICENSE_ROYALTYFREE"
}

Task Pack -Depends Collect -Description "Create NuGet packages and archive files." {
    $version = Get-PackageVersion

    Create-Archive "Flash-$version"
    
    Create-Package "Flash.Extensions.Cache" $version
    Create-Package "Flash.Extensions.Cache.Redis" $version
    Create-Package "Flash.Extensions.Cache.Redis.DependencyInjection" $version

    Create-Package "Flash.Extensions.Configuration.Json" $version
    Create-Package "Flash.Extensions.Configuration.Apollo" $version

    Create-Package "Flash.Extensions.EventBus" $version
    Create-Package "Flash.Extensions.EventBus.RabbitMQ" $version

    Create-Package "Flash.Extensions.HealthChecks" $version
    Create-Package "Flash.Extensions.HealthChecks.MySql" $version
    Create-Package "Flash.Extensions.HealthChecks.RabbitMQ" $version
    Create-Package "Flash.Extensions.HealthChecks.Redis" $version

    Create-Package "Flash.Extensions.Job" $version
    Create-Package "Flash.Extensions.Job.Hangfire" $version
    Create-Package "Flash.Extensions.Job.Hangfire.AspNetCore" $version
    Create-Package "Flash.Extensions.Job.Quartz" $version

    Create-Package "Flash.Extensions.Office" $version
    Create-Package "Flash.Extensions.Office.Npoi" $version

    Create-Package "Flash.Extensions.OpenTracting" $version
    Create-Package "Flash.Extensions.OpenTracting.Jaeger" $version
    Create-Package "Flash.Extensions.OpenTracting.Skywalking" $version

     Create-Package "Flash.Extensions.ORM" $version
    Create-Package "Flash.Extensions.ORM.EntityFrameworkCore" $version

    Create-Package "Flash.DynamicRoute" $version
    #Create-Package "Flash.Extensions.AppMetrics" $version    
    Create-Package "Flash.Extensions.DistributedLock" $version   
    Create-Package "Flash.Extensions.Resilience.Http" $version
    Create-Package "Flash.Extensions.Security" $version
    Create-Package "Flash.Extensions.UidGenerator" $version
    Create-Package "Flash.LoadBalancer" $version
    Create-Package "Flash.Extensions.Email" $version
}

function Collect-Localizations($project, $target) {
    Write-Host "Collecting localizations for '$target/$project'..." -ForegroundColor "Green"
    
    $output = (Get-SrcOutputDir $project $target)
    $dirs = Get-ChildItem -Path $output -Directory

    foreach ($dir in $dirs) {
        $source = "$output\$dir\$project.resources.dll"

        if (Test-Path $source) {
            Write-Host "  Collecting '$dir' localization..."

            $destination = "$build_dir\$target\$dir"

            Create-Directory $destination
            Copy-Files $source $destination
        }
    }
}

function Publish-Assembly($project, $target) {
    $output = Get-SrcOutputDir $project $target
    Write-Host "Publishing '$project'/$target to '$output'..." -ForegroundColor "Green"
    Exec { dotnet publish --no-build -c Release -o $output -f $target "$base_dir\src\$project" }
    Remove-Item "$output\System.*"
}

function Repack-Assembly($projectWithOptionalTarget, $internalizeAssemblies, $target) {
    $project = $projectWithOptionalTarget
    $target = $null

    $base_dir = resolve-path .
    $ilrepack = "$base_dir\packages\ilrepack.*\tools\ilrepack.exe"

    if ($projectWithOptionalTarget -Is [System.Array]) {
        $project = $projectWithOptionalTarget[0]
        $target = $projectWithOptionalTarget[1]
    }

    Write-Host "Merging '$project'/$target with $internalizeAssemblies..." -ForegroundColor "Green"

    $internalizePaths = @()

    $projectOutput = Get-SrcOutputDir $project $target

    foreach ($assembly in $internalizeAssemblies) {
        $internalizePaths += "$assembly.dll"
    }

    $primaryAssemblyPath = "$project.dll"
    $temp_dir = "$base_dir\temp"

    Create-Directory $temp_dir

    Push-Location
    Set-Location -Path $projectOutput

    Exec { .$ilrepack `
        /out:"$temp_dir\$project.dll" `
        /target:library `
        /internalize `
        $primaryAssemblyPath `
        $internalizePaths `
    }

    Pop-Location

    Move-Files "$temp_dir\$project.*" $projectOutput
}
