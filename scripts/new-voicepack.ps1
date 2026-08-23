param(
    [Parameter(Mandatory = $true)]
    [string]$PackageId,
    [Parameter(Mandatory = $true)]
    [string]$PackDefName,
    [string[]]$Actions = @('Call'),
    [string]$RaceDefName = 'Ratkin',
    [string]$ModFolderName = '',
    [string]$OutDir = (Get-Location).Path
)

Set-StrictMode -Version Latest
$ErrorActionPreference = 'Stop'

# new-voicepack.ps1 — VoicePack 脚手架（0.3.2，作者指南/SKILL 配套）。
# 生成一个独立可加载的 Race-only 最小 VoicePack：About/LoadFolders/最小 XML/音频占位目录/README。
# 只写目标目录，不碰仓库任何内容；目标已存在时 fail-fast，绝不覆盖。

$knownActions = @('Call', 'Eat', 'Sleep', 'Wounded', 'Select', 'Move', 'Social', 'Joy', 'Death',
    'Draft', 'Undraft', 'Attack', 'Work', 'Equip', 'MentalBreak', 'Crying', 'Giggling')

function Assert-Matches {
    param([string]$Value, [string]$Pattern, [string]$Label)
    if ($Value -notmatch $Pattern) {
        throw "$Label 不合法：'$Value'。要求：$Pattern"
    }
}

Assert-Matches $PackageId '^[a-z0-9][a-z0-9._-]*$' 'packageId（必须全小写，仅小写字母/数字/._-）'
Assert-Matches $PackDefName '^SR_[A-Za-z0-9_]+$' 'PackDefName（必须 SR_ 前缀，仅字母/数字/_）'
Assert-Matches $RaceDefName '^[A-Za-z0-9_]+$' 'RaceDefName（精确 ThingDef.defName）'
if ([string]::IsNullOrWhiteSpace($ModFolderName)) { $ModFolderName = $PackDefName }
Assert-Matches $ModFolderName '^[A-Za-z0-9_.-]+$' 'ModFolderName'
# 兼容 `-Actions Call,Select` 与 `-Actions @('Call','Select')` 两种传参。
# 注意：PowerShell 变量名不区分大小写，不能用 $actions 覆盖参数 $Actions。
$actionList = @()
foreach ($item in @($Actions)) {
    foreach ($part in @($item -split ',')) {
        $trimmed = $part.Trim()
        if ($trimmed -ne '') { $actionList += $trimmed }
    }
}
if ($actionList.Count -eq 0) { throw 'Actions 不能为空。' }
foreach ($action in $actionList) {
    if ($knownActions -notcontains $action) {
        throw "未知 action：$action。允许的 17 个内置键：$($knownActions -join ', ')"
    }
}
$actionList = @($actionList | Select-Object -Unique)

$packageId = $PackageId.ToLowerInvariant()
$targetRoot = Join-Path (Resolve-Path -LiteralPath $OutDir) $ModFolderName
if (Test-Path -LiteralPath $targetRoot) {
    throw "目标已存在，拒绝覆盖：$targetRoot"
}

$defsDir = Join-Path $targetRoot '1.6\Race\Defs\SoundDefs'
$soundRoot = Join-Path $targetRoot "1.6\Race\Sounds\$packageId\$PackDefName"
$null = New-Item -ItemType Directory -Force -Path $defsDir
$null = New-Item -ItemType Directory -Force -Path (Join-Path $targetRoot 'About')

# ---------------- XML
$xml = [System.Text.StringBuilder]::new()
$null = $xml.AppendLine('<?xml version="1.0" encoding="utf-8"?>')
$null = $xml.AppendLine('<Defs>')
$null = $xml.AppendLine("  <SqueakyRatkin.SqueakVoicePackDef><defName>$PackDefName</defName><scope>Race</scope><raceDefName>$RaceDefName</raceDefName><weight>1</weight><fallbacks></fallbacks><actions>")
foreach ($action in $actionList) {
    $sound = "${PackDefName}_$action"
    $null = $xml.AppendLine("    <li><action>$action</action><sounds><li>$sound</li></sounds></li>")
}
$null = $xml.AppendLine('  </actions></SqueakyRatkin.SqueakVoicePackDef>')
foreach ($action in $actionList) {
    $sound = "${PackDefName}_$action"
    $null = $xml.AppendLine("  <SoundDef><defName>$sound</defName><sustain>false</sustain><context>MapOnly</context><subSounds><li><grains><li Class=""AudioGrain_Folder""><clipFolderPath>$packageId/$PackDefName/$action</clipFolderPath></li></grains><volumeRange>45~55</volumeRange><pitchRange>0.95~1.05</pitchRange><distRange>15~70</distRange></li></subSounds></SoundDef>")
}
$null = $xml.AppendLine('</Defs>')
[System.IO.File]::WriteAllText((Join-Path $defsDir "$PackDefName.xml"), $xml.ToString(), (New-Object System.Text.UTF8Encoding($false)))

# ---------------- About / LoadFolders
$about = @"
<?xml version="1.0" encoding="utf-8"?>
<ModMetaData>
  <packageId>$packageId</packageId>
  <name>$PackDefName Voice Pack</name>
  <author>Your Name</author>
  <supportedVersions><li>1.6</li></supportedVersions>
  <description>Independent Race voice pack for Squeaky Ratkin. Replace this description, identity, and license before publishing.</description>
  <modDependencies><li><packageId>coahuilite.squeakyratkin</packageId><displayName>Squeaky Ratkin</displayName></li><li><packageId>Solaris.RatkinRaceMod</packageId><displayName>NewRatkinPlus</displayName></li></modDependencies>
  <loadAfter><li>coahuilite.squeakyratkin</li><li>Solaris.RatkinRaceMod</li></loadAfter>
</ModMetaData>
"@
[System.IO.File]::WriteAllText((Join-Path $targetRoot 'About\About.xml'), $about, (New-Object System.Text.UTF8Encoding($false)))
$loadFolders = "<?xml version=""1.0"" encoding=""utf-8""?>`n<loadFolders><v1.6><li>1.6/Race</li></v1.6></loadFolders>`n"
[System.IO.File]::WriteAllText((Join-Path $targetRoot 'LoadFolders.xml'), $loadFolders, (New-Object System.Text.UTF8Encoding($false)))

# ---------------- 音频占位目录（发布前必须替换为真实 OGG）
foreach ($action in $actionList) {
    $actionDir = Join-Path $soundRoot $action
    $null = New-Item -ItemType Directory -Force -Path $actionDir
    [System.IO.File]::WriteAllText((Join-Path $actionDir 'PUT_AUDIO_HERE.txt'),
        "Put your real .ogg clips here (recommended: OGG Vorbis, mono). Delete this file before publishing.", (New-Object System.Text.UTF8Encoding($false)))
}

# ---------------- README
$readme = @"
# $PackDefName

Generated by `scripts/new-voicepack.ps1` for Squeaky Ratkin 1.6. This is a Race-only scaffold.

- packageId: `$packageId`
- PackDef: `$PackDefName` (`raceDefName=$RaceDefName`)
- Actions declared: $($actionList -join ', ')

## 发布前必须完成
1. Replace `Your Name`, the description, and declare your own license.
2. Replace every PUT_AUDIO_HERE.txt with real, owned audio (recommended OGG Vorbis mono).
3. Keep `clipFolderPath` = `$packageId/$PackDefName/<Action>` and the real folder identical.
4. Read the full guide/skill: `.github/skills/squeaky-voicepack-authoring/SKILL.md` in the Squeaky Ratkin repo.
5. Test OFF / FALLBACK / REMIX and the fallback chain before distributing.

Note: `Crying`/`Giggling` have no built-in fallback audio; if declared here they play only when this pack provides real clips.
"@
[System.IO.File]::WriteAllText((Join-Path $targetRoot 'README.md'), $readme, (New-Object System.Text.UTF8Encoding($false)))

Write-Host "[new-voicepack] created $targetRoot"
Write-Host "  pack: $PackDefName (Race=$RaceDefName), actions: $($actionList -join ', ')"
Write-Host "  audio root: 1.6/Race/Sounds/$packageId/$PackDefName/<Action>"
Write-Host "  next: replace identity/audio/README, then enable in RimWorld after Squeaky Ratkin + NewRatkinPlus."
