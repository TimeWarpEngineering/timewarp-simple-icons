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
  Write-Host "Git Token: $RepoToken"
  git config --local user.email "github-actions-bot@timewarp.enterprises"
  git config --local user.name "GitHub Actions Bot"
  git add .
  git commit -m "Update to simple-icons version $Version"
  git push "https://github.com/$Repository.git" HEAD:main
  
  Write-Host "Successfully published changes to repository"
}
catch {
  Write-Error "Failed to publish changes: $_"
  exit 1
}
