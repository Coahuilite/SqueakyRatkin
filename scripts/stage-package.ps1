param(
    [string]$ProjectRoot = (Split-Path -Parent $PSScriptRoot),
    [Parameter(Mandatory = $true)][string]$StageDir
)

Set-StrictMode -Version Latest
$ErrorActionPreference = "Stop"

function Resolve-NormalizedPath([string]$Path) { [System.IO.Path]::GetFullPath($Path) }
function Get-RelativePath([string]$Root, [string]$Path) { $Path.Substring($Root.Length).TrimStart('\', '/').Replace('\', '/') }
function Assert-FormalExampleAudio([string]$AudioRoot, [string]$Role) {
    $actions = @('Attack','Call','Death','Draft','Eat','Equip','Joy','MentalBreak','Move','Select','Sleep','Social','Undraft','Work','Wounded')
    if (-not (Test-Path -LiteralPath $AudioRoot -PathType Container)) { throw "$Role formal audio root is missing: $AudioRoot" }
    $files = @(Get-ChildItem -LiteralPath $AudioRoot -Recurse -File)
    if (@($files | Where-Object { $_.Extension -ne '.ogg' }).Count -ne 0) { throw "$Role formal audio root contains a non-OGG file." }
    if ($files.Count -ne 41) { throw "$Role must contain exactly 41 OGG files; found $($files.Count)." }
    $foundActions = @($files | ForEach-Object { (Get-RelativePath $AudioRoot $_.FullName).Split('/')[0] } | Sort-Object -Unique)
    if (@(Compare-Object $actions $foundActions).Count -ne 0) { throw "$Role must contain audio for exactly the 15 required actions." }
    foreach ($action in $actions) {
        $expectedCount = switch ($action) {
            'Attack' { 3 }; 'Call' { 4 }; 'Death' { 2 }; 'Draft' { 3 }; 'Eat' { 2 }
            'Equip' { 2 }; 'Joy' { 3 }; 'MentalBreak' { 1 }; 'Move' { 3 }; 'Select' { 3 }
            'Sleep' { 3 }; 'Social' { 3 }; 'Undraft' { 3 }; 'Work' { 3 }; 'Wounded' { 3 }
            default { throw "No expected count is defined for action: $action" }
        }
        $actualCount = @($files | Where-Object { (Get-RelativePath $AudioRoot $_.FullName).Split('/')[0] -eq $action }).Count
        if ($actualCount -ne $expectedCount) { throw "$Role action $action must contain exactly $expectedCount OGG file(s); found $actualCount." }
    }
    $keys = @{}; foreach ($file in $files) { $relative = Get-RelativePath $AudioRoot $file.FullName; $key = (([IO.Path]::GetDirectoryName($relative).Replace('\', '/') + '/' + [IO.Path]::GetFileNameWithoutExtension($relative)).TrimStart('/')); if ($keys.ContainsKey($key)) { throw "$Role has multiple extensions for audio key: $key" }; $keys[$key] = $file }
    return $keys
}

function Assert-ExampleAudioMirrors([string]$TemplateRoot, [string]$BuiltInRoot) {
    $templateRootFull = [IO.Path]::GetFullPath($TemplateRoot).TrimEnd('\', '/')
    $builtInRootFull = [IO.Path]::GetFullPath($BuiltInRoot).TrimEnd('\', '/')
    if ($templateRootFull -eq $builtInRootFull -or $templateRootFull.StartsWith($builtInRootFull + '\', [StringComparison]::OrdinalIgnoreCase) -or $builtInRootFull.StartsWith($templateRootFull + '\', [StringComparison]::OrdinalIgnoreCase)) { throw 'Template and built-in audio roots must be distinct and neither may be an ancestor of the other.' }
    $templateKeys = Assert-FormalExampleAudio $TemplateRoot 'Template'
    $builtInKeys = Assert-FormalExampleAudio $BuiltInRoot 'Built-in mirror'
    if (@(Compare-Object $templateKeys.Keys $builtInKeys.Keys).Count -ne 0) { throw 'Template and built-in audio keys differ.' }
    foreach ($key in $templateKeys.Keys) {
        if ((Get-FileHash -LiteralPath $templateKeys[$key].FullName -Algorithm SHA256).Hash -ne (Get-FileHash -LiteralPath $builtInKeys[$key].FullName -Algorithm SHA256).Hash) { throw "Template/built-in SHA256 mismatch: $key" }
    }
}

$root = Resolve-NormalizedPath $ProjectRoot
$stageDir = Resolve-NormalizedPath $StageDir
$aboutSource = Join-Path $root 'About'
$loadFoldersSource = Join-Path $root 'LoadFolders.xml'
$versionedSource = Join-Path $root '1.6'
$extrasSource = Join-Path $root 'Extras\SqueakyRatkinExampleVoices'
$templateAudio = Join-Path $extrasSource '1.6\Race\Sounds\coahuilite.squeakyratkin.examplevoices\SR_ExampleTemplate_Race'
$builtInSourceAudio = Join-Path $versionedSource 'Sounds\coahuilite.squeakyratkin\SR_OfficialExample_Race'
$assemblyPath = Join-Path $versionedSource 'Assemblies\SqueakyRatkin.dll'

if (-not (Test-Path -LiteralPath $assemblyPath -PathType Leaf)) { throw "Missing built assembly: $assemblyPath. Build the desired flavor before staging." }
if (-not (Test-Path -LiteralPath $extrasSource -PathType Container)) { throw "Missing Template extras package: $extrasSource" }
if (Test-Path -LiteralPath $builtInSourceAudio) { throw "Unexpected built-in Example audio source exists: $builtInSourceAudio. The Template must remain the only maintained OGG source." }
Assert-FormalExampleAudio $templateAudio 'Template' | Out-Null
if (Test-Path -LiteralPath $stageDir) { Remove-Item -LiteralPath $stageDir -Recurse -Force }
$null = New-Item -ItemType Directory -Path $stageDir -Force
Copy-Item -LiteralPath $aboutSource -Destination (Join-Path $stageDir 'About') -Recurse -Force
Copy-Item -LiteralPath $loadFoldersSource -Destination (Join-Path $stageDir 'LoadFolders.xml') -Force
Copy-Item -LiteralPath $versionedSource -Destination (Join-Path $stageDir '1.6') -Recurse -Force
$null = New-Item -ItemType Directory -Path (Join-Path $stageDir 'Extras') -Force
Copy-Item -LiteralPath $extrasSource -Destination (Join-Path $stageDir 'Extras\SqueakyRatkinExampleVoices') -Recurse -Force

$stageBuiltInAudio = Join-Path $stageDir '1.6\Sounds\coahuilite.squeakyratkin\SR_OfficialExample_Race'
$null = New-Item -ItemType Directory -Path (Split-Path -Parent $stageBuiltInAudio) -Force
Copy-Item -LiteralPath $templateAudio -Destination $stageBuiltInAudio -Recurse -Force
Assert-ExampleAudioMirrors (Join-Path $stageDir 'Extras\SqueakyRatkinExampleVoices\1.6\Race\Sounds\coahuilite.squeakyratkin.examplevoices\SR_ExampleTemplate_Race') $stageBuiltInAudio

$publishedFileId = Join-Path $stageDir 'About\PublishedFileId.txt'
if (Test-Path -LiteralPath $publishedFileId) { Remove-Item -LiteralPath $publishedFileId -Force }
Get-ChildItem -LiteralPath $stageDir -Recurse -File -Filter *.pdb | Remove-Item -Force
Get-ChildItem -LiteralPath $stageDir -Recurse -File -Filter *.gitkeep | Remove-Item -Force
$fileCount = (Get-ChildItem -LiteralPath $stageDir -Recurse -File | Measure-Object).Count
Write-Host "[stage-package] Staged $fileCount files to $stageDir; Template and built-in OGG mirrors validated."
