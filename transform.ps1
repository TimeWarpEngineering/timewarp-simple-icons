Push-Location $PSScriptRoot/tools/transform
try {
    dotnet run --project transform.csproj -- "C:\git\github\simple-icons\simple-icons\icons" "C:\git\github\TimeWarpEngineering\timewarp-simple-icons\source\timewarp-simple-icons\icons"
}
finally {
    Pop-Location
}
