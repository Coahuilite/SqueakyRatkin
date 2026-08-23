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
$devDir = Join-Path $root "dist\dev"
$stageDir = Join-Path $root "dist\dev\SqueakyRatkin"
$projectFile = Join-Path $root "Source\SqueakyRatkin\SqueakyRatkin.csproj"

[xml]$projectXml = Get-Content -LiteralPath $projectFile -Raw
$versionNode = $projectXml.SelectSingleNode("/Project/PropertyGroup/Version")
if ($null -eq $versionNode -or [string]::IsNullOrWhiteSpace($versionNode.InnerText)) {
    throw "Missing <Version> in project file: $projectFile"
}

$version = $versionNode.InnerText.Trim()

# EXP 实验分支：dev 包标签与文件名带 -EXP 尾缀；About.xml/csproj 基准版本仍为 0.3.2（stage-package 按基准校验）。
$versionLabel = "$version-EXP"

try {
    $null = Get-Command -Name git -CommandType Application -ErrorAction Stop
}
catch {
    throw "Git is required to create the dev package label, but was not found on PATH."
}

$shortCommitOutput = @(& git -C $root rev-parse --short HEAD 2>$null)
if ($LASTEXITCODE -ne 0 -or $shortCommitOutput.Count -ne 1) {
    throw "Failed to determine the current Git commit for dev package labeling."
}

$shortCommit = ([string]$shortCommitOutput[0]).Trim()
if ([string]::IsNullOrWhiteSpace($shortCommit)) {
    throw "Git returned an empty commit for dev package labeling."
}

$statusOutput = @(& git -C $root status --porcelain --untracked-files=normal 2>$null)
if ($LASTEXITCODE -ne 0) {
    throw "Failed to determine whether the Git working tree is dirty for dev package labeling."
}

$dirtySuffix = if ($statusOutput.Count -gt 0) { "-dirty" } else { "" }

if (Test-Path -LiteralPath $devDir -PathType Container) {
    Get-ChildItem -LiteralPath $devDir -File -Filter "SqueakyRatkin-dev-v*.txt" | Remove-Item -Force
}
& (Join-Path $PSScriptRoot "stage-package.ps1") -ProjectRoot $root -StageDir $stageDir -VersionLabel $versionLabel -BuildFlavor dev -CommitLabel "$shortCommit$dirtySuffix" -CreateZip

$fileCount = (Get-ChildItem -LiteralPath $stageDir -Recurse -File | Measure-Object).Count
Write-Host "[pack-dev] Staged $fileCount files to $stageDir"

$labelPath = Join-Path $devDir "SqueakyRatkin-dev-v$versionLabel-$shortCommit$dirtySuffix.txt"
$null = New-Item -ItemType File -Path $labelPath -Force
Write-Host "[pack-dev] Created dev package label $labelPath"
