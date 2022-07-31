Push-Location $PSScriptRoot/tools/transform
try {
    cd $GitHub/simple-icons/simple-icons
    git pull
}
finally {
    Pop-Location
}
