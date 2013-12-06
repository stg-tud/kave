@echo off

:: this picks the first .csproj or .nuspec file in the working directory and builds a .nupkg from it
:: to package a specific project or file, add the .csproj or .nupkg file name after the pack option
:: -NoPackageAnalysis supresses warnings, caused by R# bundles not adhering to the nuget structure conventions

set TIME1=%time::=%
set TIME2=%TIME1:~0,-3%
::echo %TIME2%

set DATE1=%date:~-4,4%%date:~-7,2%%date:~-10,2%
::echo %DATE1%

set TIMESTAMP=%DATE1%-%TIME2%
::echo %TIMESTAMP%

..\.nuget\nuget pack -Verbosity detailed -NoPackageAnalysis -Version 1.0.0-v%TIMESTAMP%
