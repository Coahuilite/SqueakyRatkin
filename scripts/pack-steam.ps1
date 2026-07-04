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
$aboutSource = Join-Path $root "About"
$loadFoldersSource = Join-Path $root "LoadFolders.xml"
$versionedSource = Join-Path $root "1.6"

if ([string]::IsNullOrWhiteSpace($SteamVersion)) {
    [xml]$projectXml = Get-Content -LiteralPath $projectFile -Raw
    $semver = ($projectXml.Project.PropertyGroup.Version | Select-Object -First 1)
    $shortSha = "nogit"
    $gitSha = (& git -C $root rev-parse --short=12 HEAD 2>$null)
    if ($LASTEXITCODE -eq 0 -and -not [string]::IsNullOrWhiteSpace($gitSha)) {
        $shortSha = $gitSha.Trim()
    }

    $SteamVersion = "steam-$semver-$(Get-Date -Format 'yyyyMMdd-HHmm')-$shortSha"
}

& dotnet build $projectFile -c Release -p:SqueakyBuildFlavor=Steam -p:SqueakyInformationalVersion=$SteamVersion
if ($LASTEXITCODE -ne 0) {
    exit $LASTEXITCODE
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

# 排除占位文件(玩家包不需要 .gitkeep;音频指引 txt 保留,供翻文件的玩家查看)
Get-ChildItem -LiteralPath $stageDir -Recurse -File -Filter *.gitkeep | Remove-Item -Force

$fileCount = (Get-ChildItem -LiteralPath $stageDir -Recurse -File | Measure-Object).Count
Write-Host "[pack-steam] Staged $fileCount files to $stageDir"
Write-Host "[pack-steam] Build identity: $SteamVersion"
