param(
    [string]$ProjectRoot = (Split-Path -Parent $PSScriptRoot)
)

Set-StrictMode -Version Latest
$ErrorActionPreference = 'Stop'

# VoicePack 脚手架自检（verify-local 第 11 项）：在系统临时目录生成一个最小包，
# 校验输出结构与 XML 可解析，最后清理。只写临时目录，不碰仓库内容。

$root = [System.IO.Path]::GetFullPath($ProjectRoot)
$tempRoot = Join-Path ([System.IO.Path]::GetTempPath()) ('sr-scaffold-' + [guid]::NewGuid().ToString('N'))
try {
    $null = New-Item -ItemType Directory -Force -Path $tempRoot
    $LASTEXITCODE = 0
    & (Join-Path $PSScriptRoot 'new-voicepack.ps1') `
        -PackageId 'com.example.srscaffoldtest' `
        -PackDefName 'SR_ScaffoldTest_Race' `
        -Actions 'Call,Select' `
        -OutDir $tempRoot
    if ($LASTEXITCODE -ne 0) { exit 1 }

    $modRoot = Join-Path $tempRoot 'SR_ScaffoldTest_Race'
    $xmlPath = Join-Path $modRoot '1.6\Race\Defs\SoundDefs\SR_ScaffoldTest_Race.xml'
    if (-not (Test-Path -LiteralPath $xmlPath)) { throw 'scaffold XML was not generated.' }
    [xml]$xml = Get-Content -LiteralPath $xmlPath -Raw
    $packs = $xml.SelectNodes('/*[local-name()="Defs"]/*[local-name()="SqueakyRatkin.SqueakVoicePackDef"]')
    if ($packs.Count -ne 1) { throw 'scaffold pack count is not 1.' }
    $sounds = $xml.SelectNodes('/*[local-name()="Defs"]/*[local-name()="SoundDef"]')
    if ($sounds.Count -ne 2) { throw 'scaffold SoundDef count is not 2.' }
    foreach ($required in @('About\About.xml', 'LoadFolders.xml', 'README.md')) {
        if (-not (Test-Path -LiteralPath (Join-Path $modRoot $required))) { throw "scaffold is missing $required." }
    }

    Write-Host '[OK] VoicePack scaffold self-test: minimal Race+Call,Select pack generated and parsed.'
}
finally {
    Remove-Item -LiteralPath $tempRoot -Recurse -Force -ErrorAction SilentlyContinue
}
