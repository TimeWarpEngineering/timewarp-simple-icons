Push-Location $PSScriptRoot
try {
  . .\update.ps1
  . .\transform.ps1
  . .\publish.ps1
}
finally {
   Pop-Location
}
