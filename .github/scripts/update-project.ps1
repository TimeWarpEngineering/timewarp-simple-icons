#!/usr/bin/env pwsh

param(
  [Parameter(Mandatory = $true)]
  [string]$NewVersion
)

function Update-CsprojVersion {
  param(
    [string]$Version
  )
  
  try {
    $csprojPath = 'source/timewarp-simple-icons/timewarp-simple-icons.csproj'
    [xml]$csproj = Get-Content -Path $csprojPath
    $csproj.Project.PropertyGroup.Version = $Version
    $csproj.Save($csprojPath)
    Write-Host "Updated csproj version to $Version"
  }
  catch {
    Write-Error "Failed to update csproj: $_"
    throw
  }
}

function Update-ReleaseNotes {
  param(
    [string]$Version
  )
  
  try {
    $releaseNotes = "Update to simple-icons version $Version"
    $content = Get-Content -Path 'releases.md'
    $newContent = @(
      "# Releases",
      "",
      "## $Version",
      "",
      "* $releaseNotes",
      ""
    ) + ($content | Select-Object -Skip 1)
    $newContent | Set-Content -Path 'releases.md'
    Write-Host "Updated releases.md with version $Version"
  }
  catch {
    Write-Error "Failed to update releases.md: $_"
    throw
  }
}

try {
  Write-Host "Starting project updates for version $NewVersion"
  Update-CsprojVersion -Version $NewVersion
  Update-ReleaseNotes -Version $NewVersion
  Write-Host "Successfully completed all project updates"
}
catch {
  Write-Error "Failed to update project files: $_"
  exit 1
}
