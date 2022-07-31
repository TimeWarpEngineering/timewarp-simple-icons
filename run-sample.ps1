Push-Location $PSScriptRoot/tests/sample-app
try {
    dotnet run --project sample-app.csproj
}
finally {
    Pop-Location
}
