param(
    [string]$ProjectRoot = (Split-Path -Parent $PSScriptRoot)
)

Set-StrictMode -Version Latest
$ErrorActionPreference = "Stop"

# One-step Steam release build: build with the Steam flavor, then stage (no zip; the uploader
# publishes dist/steam/SqueakyRatkin). Steam is the final release step - pack-steam additionally
# requires a clean working tree and About.xml == csproj version, enforced by stage-package.
$root = [System.IO.Path]::GetFullPath($ProjectRoot)
$projectFile = Join-Path $root 'Source\SqueakyRatkin\SqueakyRatkin.csproj'

& dotnet build $projectFile -c Release -p:SqueakyBuildFlavor=Steam
if ($LASTEXITCODE -ne 0) {
    throw "Steam flavor build failed."
}

& (Join-Path $PSScriptRoot 'pack-steam.ps1') -ProjectRoot $root
if ($LASTEXITCODE -ne 0) {
    throw "pack-steam failed."
}
