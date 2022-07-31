Push-Location $PSScriptRoot/tests/sample-app
try {
   $Env:DOTNET_WATCH_RESTART_ON_RUDE_EDIT = "True"
    dotnet watch --project sample-app.csproj
}
finally {
    Pop-Location
}
