#!/usr/bin/env pwsh

param(
  [Parameter(Mandatory = $true)]
  [string]$Version,
  
  [Parameter(Mandatory = $true)]
  [string]$RepoToken,
  
  [Parameter(Mandatory = $true)]
  [string]$Repository
)

try {
  Write-Host "Configuring git..."
  git config --local user.email "github-actions-bot@timewarp.enterprises"
  git config --local user.name "GitHub Actions Bot"
  
  Write-Host "Adding changes..."
  git add .
  
  Write-Host "Committing changes..."
  git commit -m "Update to simple-icons version $Version"
  
  Write-Host "Pushing changes..."
  git push "https://x-access-token:$RepoToken@github.com/$Repository.git" HEAD:main
  
  Write-Host "Successfully published changes to repository"
}
catch {
  Write-Error "Failed to publish changes: $_"
  exit 1
}
