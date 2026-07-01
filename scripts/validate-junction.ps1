param(
    [string]$wsRoot = (Split-Path -Parent $PSScriptRoot),
    [string]$modName = "SqueakyRatkin"
)

Set-StrictMode -Version Latest
$ErrorActionPreference = "Stop"

function Resolve-NormalizedPath {
    param([Parameter(Mandatory = $true)][string]$Path)

    return [System.IO.Path]::GetFullPath($Path).TrimEnd('\')
}

$workspaceRoot = Resolve-NormalizedPath -Path $wsRoot
$candidates = @()

if ($env:RIMWORLD_DIR) {
    $candidates += $env:RIMWORLD_DIR
}

$candidates += @(
    "I:\SteamLibrary\steamapps\common\RimWorld\Mods",
    "C:\Program Files (x86)\Steam\steamapps\common\RimWorld\Mods"
)

$rimworldMods = $candidates | Where-Object { $_ -and (Test-Path -LiteralPath $_ -PathType Container) } | Select-Object -First 1

if (-not $rimworldMods) {
    Write-Warning "[validate-junction] RimWorld Mods folder not found. Set RIMWORLD_DIR or create the junction manually if needed."
    exit 0
}

$modLink = Join-Path $rimworldMods $modName
$expectedCommand = "New-Item -ItemType Junction -Path '$modLink' -Target '$workspaceRoot'"

if (-not (Test-Path -LiteralPath $modLink)) {
    Write-Warning "[validate-junction] Missing junction. Run: $expectedCommand"
    exit 0
}

$item = Get-Item -LiteralPath $modLink -Force
$linkType = $null
$actualTarget = $null

if ($item.PSObject.Properties.Match('LinkType').Count -gt 0) {
    $linkType = $item.LinkType
}

if ($item.PSObject.Properties.Match('Target').Count -gt 0 -and $item.Target) {
    $actualTarget = $item.Target
    if ($actualTarget -is [System.Array]) {
        $actualTarget = $actualTarget[0]
    }
}

$normalizedActualTarget = if ($actualTarget) { Resolve-NormalizedPath -Path $actualTarget } else { $null }

if ($linkType -ne 'Junction' -or $normalizedActualTarget -ne $workspaceRoot) {
    Write-Warning "[validate-junction] Junction mismatch. Run: $expectedCommand"
    exit 0
}

Write-Host "[validate-junction] OK: $modLink -> $workspaceRoot"
exit 0
