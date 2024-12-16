#!/usr/bin/env pwsh

param(
    [Parameter(Mandatory = $true)]
    [string]$WorkspacePath,
    
    [Parameter(Mandatory = $true)]
    [string]$RepoToken,
    
    [Parameter(Mandatory = $true)]
    [string]$NugetKey,
    
    [Parameter(Mandatory = $false)]
    [string]$ForcePublish = "false"
)

# Convert ForcePublish to boolean
$ForcePublish = [System.Convert]::ToBoolean($ForcePublish)
Write-Host "Force publish: $ForcePublish"

function Compare-Versions {
    try {
        # Get latest simple-icons version
        $simpleIconsRelease = Invoke-RestMethod -Uri 'https://api.github.com/repos/simple-icons/simple-icons/releases/latest' -ErrorAction Stop
        $script:SimpleIconsVersion = $simpleIconsRelease.tag_name
        
        # Get current library version from csproj
        [xml]$csproj = Get-Content -Path 'source/timewarp-simple-icons/timewarp-simple-icons.csproj'
        $script:LibraryVersion = $csproj.Project.PropertyGroup.Version
        
        Write-Host "Latest simple-icons version: $SimpleIconsVersion"
        Write-Host "Current library version: $LibraryVersion"
        
        # Explicitly return a boolean
        return $SimpleIconsVersion -ne $LibraryVersion
    }
    catch {
        Write-Error "Failed to compare versions: $_"
        exit 1
    }
}

function Update-ProjectFiles {
    param([string]$NewVersion)
    
    try {
        # Update csproj
        $csprojPath = 'source/timewarp-simple-icons/timewarp-simple-icons.csproj'
        [xml]$csproj = Get-Content -Path $csprojPath
        $csproj.Project.PropertyGroup.Version = $NewVersion
        $csproj.Save($csprojPath)
        
        # Update releases.md
        $releaseNotes = "Update to simple-icons version $NewVersion"
        $content = Get-Content -Path 'releases.md'
        $newContent = @(
            "# Releases",
            "",
            "## $NewVersion",
            "",
            "* $releaseNotes",
            ""
        ) + ($content | Select-Object -Skip 1)
        $newContent | Set-Content -Path 'releases.md'
        
        Write-Host "Updated project files to version $NewVersion"
    }
    catch {
        Write-Error "Failed to update project files: $_"
        exit 1
    }
}

function Update-Icons {
    try {
        Write-Host "Cloning simple-icons repository..."
        git clone --depth 1 https://github.com/simple-icons/simple-icons.git my-tools
        
        Write-Host "Running transform tool..."
        Push-Location tools\transform
        try {
            dotnet run --project transform.csproj -- "$WorkspacePath/my-tools/icons" "$WorkspacePath/source/timewarp-simple-icons/icons"
        }
        finally {
            Pop-Location
        }
        
        Write-Host "Cleaning up cloned repository..."
        Remove-Item -Path "my-tools" -Recurse -Force
        Write-Host "Icon update completed"
    }
    catch {
        Write-Error "Failed to update icons: $_"
        if (Test-Path "my-tools") {
            Remove-Item -Path "my-tools" -Recurse -Force -ErrorAction SilentlyContinue
        }
        exit 1
    }
}

function Push-Changes {
    try {
        Write-Host "Configuring git..."
        git config --local user.email "github-actions-bot@timewarp.enterprises"
        git config --local user.name "GitHub Actions Bot"
        
        Write-Host "Adding changes..."
        git add .
        
        Write-Host "Committing changes..."
        git commit -m "Update to simple-icons version $SimpleIconsVersion"
        
        Write-Host "Pushing changes..."
        git push "https://x-access-token:$RepoToken@github.com/$env:GITHUB_REPOSITORY.git" HEAD:main
    }
    catch {
        Write-Error "Failed to push changes: $_"
        exit 1
    }
}

function Publish-ToNuGet {
    try {
        $script:Nuget_Key = $NugetKey
        Push-Location $WorkspacePath
        try {
            ./publish.ps1
        }
        finally {
            Pop-Location
        }
    }
    catch {
        Write-Error "Failed to publish to NuGet: $_"
        exit 1
    }
}

# Main execution
try {
    $needsUpdate = Compare-Versions
    Write-Host "Needs update: $needsUpdate"
    
    if ($needsUpdate) {
        Write-Host "New version detected. Processing release..."
        Update-ProjectFiles -NewVersion $SimpleIconsVersion
        Update-Icons
        Push-Changes
        Publish-ToNuGet
        Write-Host "Release processing completed successfully"
    }
    elseif ($ForcePublish) {
        Write-Host "Force publish requested. Processing release..."
        Update-ProjectFiles -NewVersion $SimpleIconsVersion
        Update-Icons
        Push-Changes
        Publish-ToNuGet
        Write-Host "Release processing completed successfully"
    }
    else {
        Write-Host "No update needed. Current version is up to date."
        exit 0
    }
}
catch {
    Write-Error "Release processing failed: $_"
    exit 1
}
