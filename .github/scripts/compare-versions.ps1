#!/usr/bin/env pwsh

try {
  # Get latest simple-icons version
  $simpleIconsRelease = Invoke-RestMethod -Uri 'https://api.github.com/repos/simple-icons/simple-icons/releases/latest' -ErrorAction Stop
  $simpleIconsVersion = $simpleIconsRelease.tag_name
  
  # Get current library version from csproj
  [xml]$csproj = Get-Content -Path 'source/timewarp-simple-icons/timewarp-simple-icons.csproj'
  $libVersion = $csproj.Project.PropertyGroup.Version
  
  # Output versions for GitHub Actions
  "si=$simpleIconsVersion" >> $env:GITHUB_OUTPUT
  "lib=$libVersion" >> $env:GITHUB_OUTPUT
  
  # Log versions for visibility
  Write-Host "Latest simple-icons version: $simpleIconsVersion"
  Write-Host "Current library version: $libVersion"
}
catch {
  Write-Error "Failed to compare versions: $_"
  exit 1
}
