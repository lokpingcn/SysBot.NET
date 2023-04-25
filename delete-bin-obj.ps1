Get-ChildItem -Path . -Directory -Recurse | ForEach-Object {
    $binDir = Join-Path $_.FullName "bin"
    $objDir = Join-Path $_.FullName "obj"
    if (Test-Path $binDir) {
        Remove-Item $binDir -Recurse -Force
    }
    if (Test-Path $objDir) {
        Remove-Item $objDir -Recurse -Force
    }
}