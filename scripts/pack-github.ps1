param(
    [string]$ProjectRoot = (Split-Path -Parent $PSScriptRoot),
    [string]$Version = (Get-Date -Format "yyyyMMdd-HHmmss")
)

Set-StrictMode -Version Latest
$ErrorActionPreference = "Stop"

function Resolve-NormalizedPath {
    param([Parameter(Mandatory = $true)][string]$Path)

    return [System.IO.Path]::GetFullPath($Path)
}

$root = Resolve-NormalizedPath -Path $ProjectRoot
$stageDir = Join-Path $root "dist\github\SqueakyRatkin"
$zipDir = Join-Path $root "dist\github"
$dllPath = Join-Path $root "1.6\Assemblies\SqueakyRatkin.dll"

& (Join-Path $PSScriptRoot "stage-package.ps1") -ProjectRoot $root -StageDir $stageDir

$resolvedVersion = $Version
if (-not $PSBoundParameters.ContainsKey('Version') -and (Test-Path -LiteralPath $dllPath -PathType Leaf)) {
    $assemblyVersion = [System.Reflection.AssemblyName]::GetAssemblyName($dllPath).Version
    if ($assemblyVersion) {
        $resolvedVersion = $assemblyVersion.ToString()
    }
}

$zipPath = Join-Path $zipDir "SqueakyRatkin-$resolvedVersion.zip"
if (Test-Path -LiteralPath $zipPath) {
    Remove-Item -LiteralPath $zipPath -Force
}

Compress-Archive -Path $stageDir -DestinationPath $zipPath -Force

$fileCount = (Get-ChildItem -LiteralPath $stageDir -Recurse -File | Measure-Object).Count
Write-Host "[pack-github] Staged $fileCount files to $stageDir"
Write-Host "[pack-github] Created $zipPath"
