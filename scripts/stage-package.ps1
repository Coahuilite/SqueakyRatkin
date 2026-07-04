param(
    [string]$ProjectRoot = (Split-Path -Parent $PSScriptRoot),
    [Parameter(Mandatory = $true)][string]$StageDir
)

Set-StrictMode -Version Latest
$ErrorActionPreference = "Stop"

function Resolve-NormalizedPath {
    param([Parameter(Mandatory = $true)][string]$Path)

    return [System.IO.Path]::GetFullPath($Path)
}

$root = Resolve-NormalizedPath -Path $ProjectRoot
$stageDir = Resolve-NormalizedPath -Path $StageDir
$aboutSource = Join-Path $root "About"
$loadFoldersSource = Join-Path $root "LoadFolders.xml"
$versionedSource = Join-Path $root "1.6"
$assemblyPath = Join-Path $versionedSource "Assemblies\SqueakyRatkin.dll"

if (-not (Test-Path -LiteralPath $assemblyPath -PathType Leaf)) {
    throw "Missing built assembly: $assemblyPath. Build the desired flavor before staging."
}

if (Test-Path -LiteralPath $stageDir) {
    Remove-Item -LiteralPath $stageDir -Recurse -Force
}

$null = New-Item -ItemType Directory -Path $stageDir -Force
Copy-Item -LiteralPath $aboutSource -Destination (Join-Path $stageDir "About") -Recurse -Force
Copy-Item -LiteralPath $loadFoldersSource -Destination (Join-Path $stageDir "LoadFolders.xml") -Force
Copy-Item -LiteralPath $versionedSource -Destination (Join-Path $stageDir "1.6") -Recurse -Force

$publishedFileId = Join-Path $stageDir "About\PublishedFileId.txt"
if (Test-Path -LiteralPath $publishedFileId) {
    Remove-Item -LiteralPath $publishedFileId -Force
}

Get-ChildItem -LiteralPath $stageDir -Recurse -File -Filter *.pdb | Remove-Item -Force
Get-ChildItem -LiteralPath $stageDir -Recurse -File -Filter *.gitkeep | Remove-Item -Force

$fileCount = (Get-ChildItem -LiteralPath $stageDir -Recurse -File | Measure-Object).Count
Write-Host "[stage-package] Staged $fileCount files to $stageDir"
