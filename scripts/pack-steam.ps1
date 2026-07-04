param(
    [string]$ProjectRoot = (Split-Path -Parent $PSScriptRoot),
    [string]$SteamVersion = ""
)

Set-StrictMode -Version Latest
$ErrorActionPreference = "Stop"

function Resolve-NormalizedPath {
    param([Parameter(Mandatory = $true)][string]$Path)

    return [System.IO.Path]::GetFullPath($Path)
}

$root = Resolve-NormalizedPath -Path $ProjectRoot
$projectFile = Join-Path $root "Source\SqueakyRatkin\SqueakyRatkin.csproj"
$stageDir = Join-Path $root "dist\steam\SqueakyRatkin"

if ([string]::IsNullOrWhiteSpace($SteamVersion)) {
    [xml]$projectXml = Get-Content -LiteralPath $projectFile -Raw
    $SteamVersion = ($projectXml.Project.PropertyGroup.Version | Select-Object -First 1)
}

& (Join-Path $PSScriptRoot "stage-package.ps1") -ProjectRoot $root -StageDir $stageDir

$fileCount = (Get-ChildItem -LiteralPath $stageDir -Recurse -File | Measure-Object).Count
Write-Host "[pack-steam] Staged $fileCount files to $stageDir"
Write-Host "[pack-steam] Build identity: $SteamVersion"
