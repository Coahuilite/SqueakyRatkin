param(
    [string]$ProjectRoot = (Split-Path -Parent $PSScriptRoot)
)

Set-StrictMode -Version Latest
$ErrorActionPreference = "Stop"

# One-step local dev package: build with the Dev flavor (csproj default), then stage + zip.
# Flavor is guaranteed by construction - pack-dev never sees a foreign-flavor DLL from this entry.
$root = [System.IO.Path]::GetFullPath($ProjectRoot)
$projectFile = Join-Path $root 'Source\SqueakyRatkin\SqueakyRatkin.csproj'

& dotnet build $projectFile -c Release
if ($LASTEXITCODE -ne 0) {
    throw "Dev flavor build failed."
}

& (Join-Path $PSScriptRoot 'pack-dev.ps1') -ProjectRoot $root
if ($LASTEXITCODE -ne 0) {
    throw "pack-dev failed."
}
