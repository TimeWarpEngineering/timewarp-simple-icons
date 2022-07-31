Push-Location $PSScriptRoot/tools/transform
try {
  update.ps1
  transform.ps1
  publish.ps1
}
finally {
   Pop-Location
}
