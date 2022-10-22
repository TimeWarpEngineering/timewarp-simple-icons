Push-Location $PSScriptRoot/tools/transform
try {
    dotnet run --project transform.csproj -- "J:\Open Source\simple-icons\icons" "J:\Open Source\timewarp-simple-icons\Source\timewarp-simple-icons\icons"
}
finally {
    Pop-Location
}
