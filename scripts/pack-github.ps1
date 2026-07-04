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
$projectFile = Join-Path $root "Source\SqueakyRatkin\SqueakyRatkin.csproj"
$stageDir = Join-Path $root "dist\github\SqueakyRatkin"
$zipDir = Join-Path $root "dist\github"
$aboutSource = Join-Path $root "About"
$loadFoldersSource = Join-Path $root "LoadFolders.xml"
$versionedSource = Join-Path $root "1.6"
$dllPath = Join-Path $root "1.6\Assemblies\SqueakyRatkin.dll"

& dotnet build $projectFile -c Release -p:SqueakyBuildFlavor=GitHub -p:SqueakyInformationalVersion=$Version
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
