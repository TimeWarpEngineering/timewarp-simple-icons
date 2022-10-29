Push-Location $PSScriptRoot/tools/transform
try {
  if (!$simple_icons) { throw "simple_icons should be set to the root path of where you cloned the simple-icons repo"}
  dotnet run --project transform.csproj -- "$simple_icons\icons" "$PSScriptRoot\Source\timewarp-simple-icons\icons"
}
finally {
    Pop-Location
}
