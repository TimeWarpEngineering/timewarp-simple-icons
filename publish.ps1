Push-Location $PSScriptRoot
try {
    if (!$Nuget_Key) { throw "Nuget_Key is not set"}
    dotnet tool restore
    dotnet cleanup -y
    dotnet pack ./source/timewarp-simple-icons/timewarp-simple-icons.csproj -c Release --output packages
    Push-Location ./packages
    dotnet nuget push **/*.nupkg --source https://api.nuget.org/v3/index.json --api-key $Nuget_Key
    Pop-Location
}
finally {
    Pop-Location
}
