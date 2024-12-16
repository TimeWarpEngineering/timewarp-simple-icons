#!/usr/bin/env pwsh

param(
  [Parameter(Mandatory = $true)]
  [string]$WorkspacePath
)

function Clone-SimpleIcons {
  param(
    [string]$Path
  )
  
  try {
    Write-Host "Cloning simple-icons repository..."
    git clone --depth 1 https://github.com/simple-icons/simple-icons.git $Path
    Write-Host "Successfully cloned simple-icons repository"
    
    Get-ChildItem -Path $Path | Format-Table Name
  }
  catch {
    Write-Error "Failed to clone simple-icons repository: $_"
    throw
  }
}

function Update-Icons {
  param(
    [string]$SourcePath,
    [string]$DestinationPath
  )
  
  try {
    Write-Host "Running transform tool to update icons..."
    Push-Location tools\transform
    try {
      dotnet run --project transform.csproj -- $SourcePath $DestinationPath
    }
    finally {
      Pop-Location
    }
    Write-Host "Successfully updated icons"
  }
  catch {
    Write-Error "Failed to update icons: $_"
    throw
  }
}

function Remove-ClonedRepository {
  param(
    [string]$Path
  )
  
  try {
    Write-Host "Cleaning up cloned repository..."
    Remove-Item -Path $Path -Recurse -Force
    Write-Host "Successfully cleaned up cloned repository"
  }
  catch {
    Write-Error "Failed to clean up cloned repository: $_"
    throw
  }
}

try {
  $repoPath = Join-Path -Path $WorkspacePath -ChildPath "my-tools"
  
  Clone-SimpleIcons -Path $repoPath
  Update-Icons -SourcePath "$repoPath/icons" -DestinationPath "$WorkspacePath/source/timewarp-simple-icons/icons"
  Remove-ClonedRepository -Path $repoPath
  
  Write-Host "Icon update process completed successfully"
}
catch {
  Write-Error "Icon update process failed: $_"
  if (Test-Path $repoPath) {
    Write-Host "Attempting to clean up repository directory..."
    Remove-Item -Path $repoPath -Recurse -Force -ErrorAction SilentlyContinue
  }
  exit 1
}
