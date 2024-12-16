Push-Location $PSScriptRoot
try {
    # Check for PowerShell variable first, then environment variable
    if (!$Nuget_Key) { 
        $Nuget_Key = $env:Nuget_Key
    }
    if (!$Nuget_Key) { 
        throw "Nuget_Key is not set in either variable scope"
    }

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
