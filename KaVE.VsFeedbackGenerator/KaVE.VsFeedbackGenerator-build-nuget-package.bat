# this picks the first .csproj or .nuspec file in the working directory and builds a .nupkg from it
# to package a specific project or file, add the .csproj or .nupkg file name after the pack option
# -NoPackageAnalysis supresses warnings, caused by R# bundles not adhering to the nuget structure conventions
..\.nuget\nuget pack -Verbosity detailed -NoPackageAnalysis