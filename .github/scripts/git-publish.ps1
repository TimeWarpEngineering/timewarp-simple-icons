#!/usr/bin/env pwsh

param(
  [Parameter(Mandatory = $true)]
  [string]$Version
)

try {
  Write-Host "Configuring git..."
  git config --local user.email "github-actions-bot@timewarp.enterprises"
  git config --local user.name "GitHub Actions Bot"
  
  Write-Host "Adding changes..."
  git add .
  
  Write-Host "Committing changes..."
  git commit -m "Update to simple-icons version $Version"
  
  Write-Host "Changes committed successfully"
}
catch {
  Write-Error "Failed to prepare changes: $_"
  exit 1
}
