[CmdletBinding()]
param(
    [Parameter(Mandatory = $true)][ValidateNotNullOrEmpty()][string]$Version,
    [Parameter(Mandatory = $true)][ValidateNotNullOrEmpty()][string]$OutputPath
)

Set-StrictMode -Version Latest
$ErrorActionPreference = 'Stop'

$semVerPattern = '^(?:v)?(?<base>(?:0|[1-9]\d*)\.(?:0|[1-9]\d*)\.(?:0|[1-9]\d*))(?:-(?:(?:0|[1-9]\d*)|[0-9A-Za-z-]*[A-Za-z-][0-9A-Za-z-]*)(?:\.(?:(?:0|[1-9]\d*)|[0-9A-Za-z-]*[A-Za-z-][0-9A-Za-z-]*))*)?$'
$versionMatch = [regex]::Match($Version.Trim(), $semVerPattern)
if (-not $versionMatch.Success) { throw 'Version must be a strict SemVer 2.0 tag or bare version.' }
$baseVersion = $versionMatch.Groups['base'].Value
$versionToken = [regex]::Escape($baseVersion)

function Get-ChangelogEntry([string]$Path, [string]$VersionToken, [string]$Language) {
    if (-not (Test-Path -LiteralPath $Path -PathType Leaf)) { throw "$Language changelog is missing." }
    $lines = [IO.File]::ReadAllLines($Path, [Text.Encoding]::UTF8)
    $headings = New-Object 'System.Collections.Generic.List[int]'
    for ($i = 0; $i -lt $lines.Length; $i++) {
        if ($lines[$i] -match '^##\s+') { $headings.Add($i) }
    }

    $matchingHeadings = New-Object 'System.Collections.Generic.List[int]'
    $versionPattern = '(?<![A-Za-z0-9])' + $VersionToken + '(?![A-Za-z0-9])'
    foreach ($index in $headings) {
        $heading = $lines[$index]
        $hasVersion = [regex]::IsMatch($heading, $versionPattern)
        $isUnreleased = $heading -match '(?i)Unreleased|未发布'
        $isDatedRelease = $heading -match '(?i)\bVersion\s+' + $versionPattern -or $heading -match '版本\s*' + $versionPattern
        if ($hasVersion -and ($isUnreleased -or $isDatedRelease)) { $matchingHeadings.Add($index) }
    }
    if ($matchingHeadings.Count -eq 0) { throw "$Language changelog has no entry for version $VersionToken." }
    if ($matchingHeadings.Count -gt 1) { throw "$Language changelog has multiple entries for version $VersionToken." }

    $start = $matchingHeadings[0] + 1
    $end = $lines.Length
    foreach ($index in $headings) { if ($index -gt $matchingHeadings[0]) { $end = $index; break } }
    if ($start -ge $end) { throw "$Language changelog entry for version $VersionToken has no body." }
    $body = ($lines[$start..($end - 1)] -join "`n").Trim()
    if ([string]::IsNullOrWhiteSpace($body)) { throw "$Language changelog entry for version $VersionToken has no body." }
    return $body
}

$docsRoot = [IO.Path]::GetFullPath((Join-Path $PSScriptRoot '..\docs'))
$english = Get-ChangelogEntry (Join-Path $docsRoot 'CHANGELOG.md') $versionToken 'English'
$chinese = Get-ChangelogEntry (Join-Path $docsRoot 'CHANGELOG.zh-CN.md') $versionToken 'Chinese'
$displayVersion = $Version.Trim()
$content = "# Squeaky Ratkin $displayVersion`n`n## English`n`n$english`n`n## 中文`n`n$chinese`n"
$destination = [IO.Path]::GetFullPath($OutputPath)
$parent = Split-Path -Parent $destination
if (-not [string]::IsNullOrWhiteSpace($parent)) { [IO.Directory]::CreateDirectory($parent) | Out-Null }
[IO.File]::WriteAllText($destination, $content, [Text.UTF8Encoding]::new($false))
Write-Host "[release-notes] Prepared bilingual notes for $displayVersion."
