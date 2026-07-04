param(
    [string]$ProjectRoot = (Split-Path -Parent $PSScriptRoot)
)

Set-StrictMode -Version Latest
$ErrorActionPreference = "Stop"

function Resolve-NormalizedPath {
    param([Parameter(Mandatory = $true)][string]$Path)

    return [System.IO.Path]::GetFullPath($Path)
}

$root = Resolve-NormalizedPath -Path $ProjectRoot
$stageDir = Join-Path $root "dist\dev\SqueakyRatkin"

& (Join-Path $PSScriptRoot "stage-package.ps1") -ProjectRoot $root -StageDir $stageDir

$fileCount = (Get-ChildItem -LiteralPath $stageDir -Recurse -File | Measure-Object).Count
Write-Host "[pack-dev] Staged $fileCount files to $stageDir"
