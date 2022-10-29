if (!$simple_icons) { throw "simple_icons should be set to the root path of where you cloned the simple-icons repo"}
Push-Location $simple_icons
try {
  git pull
}
finally {
    Pop-Location
}
