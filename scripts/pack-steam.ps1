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
$projectFile = Join-Path $root "Source\SqueakyRatkin\SqueakyRatkin.csproj"
$stageDir = Join-Path $root "dist\steam\SqueakyRatkin"

if (-not (Test-Path -LiteralPath $projectFile -PathType Leaf)) {
    throw "Missing project file: $projectFile"
}

[xml]$projectXml = Get-Content -LiteralPath $projectFile -Raw
$versionNodes = @($projectXml.SelectNodes("/Project/PropertyGroup/Version"))
if ($versionNodes.Count -ne 1 -or [string]::IsNullOrWhiteSpace($versionNodes[0].InnerText)) {
    throw "Project file must contain exactly one non-empty <Version>: $projectFile"
}
$version = $versionNodes[0].InnerText.Trim()

try {
    $null = Get-Command -Name git -CommandType Application -ErrorAction Stop
}
catch {
    throw "Git is required to create the package label, but was not found on PATH."
}
$shortCommitOutput = @(& git -C $root rev-parse --short HEAD 2>$null)
if ($LASTEXITCODE -ne 0 -or $shortCommitOutput.Count -ne 1) {
    throw "Failed to determine the current Git commit for package labeling."
}
$shortCommit = ([string]$shortCommitOutput[0]).Trim()

# Steam is the final publish step: it must be clean AND Steam-release-state. A package built from
# uncommitted changes would mislabel provenance (commit=<HEAD sha>) and bypass review gates.
$statusOutput = @(& git -C $root status --porcelain --untracked-files=normal 2>$null)
if ($LASTEXITCODE -ne 0) {
    throw "Failed to determine whether the Git working tree is clean for Steam packaging."
}
if ($statusOutput.Count -gt 0) {
    throw "Steam packaging requires a clean working tree (uncommitted changes present). Commit first - Steam is the final release step."
}

& (Join-Path $PSScriptRoot "stage-package.ps1") -ProjectRoot $root -StageDir $stageDir -VersionLabel $version -BuildFlavor steam -CommitLabel $shortCommit

$fileCount = (Get-ChildItem -LiteralPath $stageDir -Recurse -File | Measure-Object).Count
Write-Host "[pack-steam] Staged $fileCount files to $stageDir"
Write-Host "[pack-steam] Build identity from project <Version>: $version"
