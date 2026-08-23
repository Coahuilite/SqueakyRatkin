param(
    [string]$ProjectRoot = (Split-Path -Parent $PSScriptRoot)
)

Set-StrictMode -Version Latest
$ErrorActionPreference = "Stop"

# VoicePack XML ABI consistency lock (0.3.2, verify-local check 10).
# 三向对照：
#   XML 面   = 官方内置示例 + Extras 作者模板（提交态），彩蛋测试包为 dist/ 可选工件（存在才并入检查）
#   validator = Source/SqueakyRatkin/SqueakVoicePackModels.cs 的规则标记必须齐全
#   C# 动作源 = Source/SqueakyRatkin/SqueakActionModel.cs 的 17 键顺序（XML 与指南都对照它）
#   author guide = .github/skills/squeaky-voicepack-authoring/SKILL.md（作者指南唯一正文，兼作 agent skill）必须覆盖全部键与规则标记
# 任一不一致即 FAIL；本脚本不写任何文件。

$root = [System.IO.Path]::GetFullPath($ProjectRoot)
$failures = @()

function Assert-True {
    param([bool]$Condition, [string]$Message)
    if (-not $Condition) { $script:failures += $Message }
}

# ---------------------------------------------------------------- C# action-key source
$actionModelPath = Join-Path $root 'Source\SqueakyRatkin\SqueakActionModel.cs'
$actionModel = [System.IO.File]::ReadAllText($actionModelPath)
$actionMatches = [regex]::Matches($actionModel, 'new\(SqueakAction\.([A-Za-z0-9_]+),')
$actionKeys = @($actionMatches | ForEach-Object { $_.Groups[1].Value })
$expectedKeys = @('Call','Eat','Sleep','Wounded','Select','Move','Social','Joy','Death','Draft','Undraft','Attack','Work','Equip','MentalBreak','Crying','Giggling')
Assert-True ($actionKeys.Count -eq 17) "C# action-key parse expected 17 keys, got $($actionKeys.Count)."
Assert-True (($actionKeys -join ',') -eq ($expectedKeys -join ',')) "C# SqueakActionDefinitions order drifted from the frozen 17-key ABI: $($actionKeys -join ',')"
$exampleKeys = @($actionKeys | Where-Object { $_ -ne 'Crying' -and $_ -ne 'Giggling' })

# ---------------------------------------------------------------- XML samples
$xmlFiles = @(
    '1.6\Defs\SoundDefs\SqueakyRatkin_OfficialExample_Race.xml',
    'Extras\SqueakyRatkinExampleVoices\1.6\Race\Defs\SoundDefs\SqueakyRatkin_ExampleTemplate_Race.xml'
)
$eggTestXml = 'dist\SqueakyRatkinEggTestVoices\1.6\Race\Defs\SoundDefs\SqueakyRatkin_EggTest_Race.xml'
$eggTestAbsent = -not (Test-Path -LiteralPath (Join-Path $root $eggTestXml))
if ($eggTestAbsent) {
    Write-Host '[note] Egg test pack absent (dist artifact, gitignored): core ABI lock runs without it.'
}
else {
    $xmlFiles += $eggTestXml
}

$globalPackNames = @{}
$globalSoundNames = @{}
$exampleActionSets = @()

foreach ($relative in $xmlFiles) {
    $path = Join-Path $root $relative
    Assert-True (Test-Path -LiteralPath $path) "Missing VoicePack XML sample: $relative"
    if (-not (Test-Path -LiteralPath $path)) { continue }

    [xml]$xml = [System.IO.File]::ReadAllText($path)
    $packs = $xml.SelectNodes('/*[local-name()="Defs"]/*[local-name()="SqueakyRatkin.SqueakVoicePackDef"]')
    Assert-True ($packs.Count -eq 1) "$relative expected exactly 1 SqueakVoicePackDef, got $($packs.Count)."
    if ($packs.Count -ne 1) { continue }

    $pack = $packs[0]
    $defName = $pack.SelectSingleNode('defName').InnerText
    Assert-True ($defName.StartsWith('SR_')) "$relative PackDef $defName must start with SR_."
    if ($globalPackNames.ContainsKey($defName)) { Assert-True $false "$relative duplicates PackDef $defName (also in $($globalPackNames[$defName]))." }
    else { $globalPackNames[$defName] = $relative }

    $scope = $pack.SelectSingleNode('scope').InnerText
    Assert-True ($scope -eq 'Race') "$relative PackDef $defName scope must be Race."

    $race = $pack.SelectSingleNode('raceDefName').InnerText
    Assert-True ($race -eq 'Ratkin') "$relative PackDef $defName raceDefName must be Ratkin (exact, case-sensitive)."

    $weightText = $pack.SelectSingleNode('weight').InnerText
    $weight = 0.0
    Assert-True ([double]::TryParse($weightText, [ref]$weight)) "$relative PackDef $defName weight is not numeric."
    Assert-True ($weight -gt 0 -and -not [double]::IsNaN($weight) -and -not [double]::IsInfinity($weight)) "$relative PackDef $defName weight must be finite and greater than zero."

    $actions = $pack.SelectNodes('actions/li')
    Assert-True ($actions.Count -ge 1) "$relative PackDef $defName has no action entries."
    $seen = @{}
    $packActions = @()
    $soundRefs = @()
    $hasEgg = $false

    foreach ($entry in $actions) {
        $action = $entry.SelectSingleNode('action').InnerText
        Assert-True ($actionKeys -contains $action) "$relative PackDef $defName has unknown action $action."
        if ($actionKeys -contains $action) { $packActions += $action }

        $ageTagNode = $entry.SelectSingleNode('ageTag')
        $ageTag = if ($null -ne $ageTagNode) { $ageTagNode.InnerText } else { '' }
        if ($ageTag -ne '') {
            Assert-True (@('Baby','Toddler','Child','Adult') -contains $ageTag) "$relative PackDef $defName action $action has unknown ageTag $ageTag."
        }
        $ageKey = if ($ageTag -eq '') { 'all-age' } else { $ageTag }
        $dupKey = "$action|$ageKey"
        if ($seen.ContainsKey($dupKey)) { Assert-True $false "$relative PackDef $defName duplicates action $action for ageTag $ageKey." }
        else { $seen[$dupKey] = $true }

        $eggNode = $entry.SelectSingleNode('IsEgg')
        if ($null -ne $eggNode) {
            $eggText = $eggNode.InnerText
            Assert-True ($eggText -eq 'true' -or $eggText -eq 'false') "$relative PackDef $defName action $action IsEgg must be true or false."
            if ($eggText -eq 'true') { $hasEgg = $true }
        }

        $sounds = $entry.SelectNodes('sounds/li')
        Assert-True ($sounds.Count -ge 1) "$relative PackDef $defName action $action has no sounds."
        foreach ($soundRef in $sounds) {
            $soundName = $soundRef.InnerText
            Assert-True ($soundName.StartsWith('SR_')) "$relative PackDef $defName action $action references SoundDef without SR_ prefix: $soundName."
            $soundRefs += $soundName
        }
    }

    foreach ($fallback in $pack.SelectNodes('fallbacks/li')) {
        $fallbackAction = $fallback.SelectSingleNode('action').InnerText
        Assert-True ($actionKeys -contains $fallbackAction) "$relative PackDef $defName fallback has unknown action $fallbackAction."
        $fallbackSound = $fallback.SelectSingleNode('sound').InnerText
        Assert-True ($fallbackSound.StartsWith('SR_')) "$relative PackDef $defName fallback $fallbackAction references SoundDef without SR_ prefix: $fallbackSound."
        $soundRefs += $fallbackSound
    }

    $soundDefs = $xml.SelectNodes('/*[local-name()="Defs"]/*[local-name()="SoundDef"]')
    $soundNamesInFile = @{}
    foreach ($sound in $soundDefs) {
        $soundName = $sound.SelectSingleNode('defName').InnerText
        Assert-True ($soundName.StartsWith('SR_')) "$relative SoundDef $soundName must start with SR_."
        if ($globalSoundNames.ContainsKey($soundName)) { Assert-True $false "$relative duplicates SoundDef $soundName (also in $($globalSoundNames[$soundName]))." }
        else { $globalSoundNames[$soundName] = $relative }
        $soundNamesInFile[$soundName] = $true

        $sustainNode = $sound.SelectSingleNode('sustain')
        if ($null -ne $sustainNode) {
            Assert-True ($sustainNode.InnerText -eq 'false') "$relative SoundDef $soundName must have sustain=false."
        }
        $contextNode = $sound.SelectSingleNode('context')
        Assert-True ($null -ne $contextNode -and $contextNode.InnerText -eq 'MapOnly') "$relative SoundDef $soundName must have context=MapOnly."

        $subSounds = $sound.SelectNodes('subSounds/li')
        Assert-True ($subSounds.Count -ge 1) "$relative SoundDef $soundName has no SubSounds."
        foreach ($sub in $subSounds) {
            $onCameraNode = $sub.SelectSingleNode('onCamera')
            if ($null -ne $onCameraNode) {
                Assert-True ($onCameraNode.InnerText -eq 'false') "$relative SoundDef $soundName has an onCamera SubSound."
            }
            $grains = $sub.SelectNodes('grains/li')
            Assert-True ($grains.Count -ge 1) "$relative SoundDef $soundName has a SubSound without grains."
            foreach ($grain in $grains) {
                $grainType = $grain.GetAttribute('Class')
                Assert-True ($grainType -eq 'AudioGrain_Folder') "$relative SoundDef $soundName grain Class must be AudioGrain_Folder."
                $clipPath = $grain.SelectSingleNode('clipFolderPath').InnerText
                Assert-True ($clipPath -like 'coahuilite.squeakyratkin*') "$relative SoundDef $soundName clipFolderPath does not match its coahuilite package root: $clipPath."
            }
        }
    }

    foreach ($soundRef in $soundRefs) {
        Assert-True $soundNamesInFile.ContainsKey($soundRef) "$relative references missing SoundDef $soundRef."
    }

    if ($relative -like '*EggTest*') {
        Assert-True $hasEgg "$relative must carry at least one IsEgg=true action (egg test pack)."
        Assert-True (($packActions -join ',') -eq 'Select') "$relative egg test pack should only declare Select, got $($packActions -join ',')."
    }
    else {
        $exampleActionSets += ,@($packActions | Sort-Object -Unique)
        Assert-True (($packActions | Sort-Object -Unique).Count -eq 15) "$relative PackDef $defName should carry the 15 shipped action keys, got $((($packActions | Sort-Object -Unique) -join ','))."
    }
}

if ($exampleActionSets.Count -eq 2) {
    $sortedExampleKeys = @($exampleKeys | Sort-Object)
    Assert-True (($exampleActionSets[0] -join ',') -eq ($sortedExampleKeys -join ',')) "Official Example XML action set drifted from the 15 shipped keys."
    Assert-True (($exampleActionSets[1] -join ',') -eq ($sortedExampleKeys -join ',')) "Extras Template XML action set drifted from the 15 shipped keys."
    Assert-True (($exampleActionSets[0] -join ',') -eq ($exampleActionSets[1] -join ',')) "Official Example and Extras Template action sets differ."
}
else {
    Assert-True $false "Expected two non-egg example XML files, found $($exampleActionSets.Count)."
}

# ---------------------------------------------------------------- validator rule markers
$validatorPath = Join-Path $root 'Source\SqueakyRatkin\SqueakVoicePackModels.cs'
$validatorText = [System.IO.File]::ReadAllText($validatorPath)
$validatorMarkers = @(
    'defName must begin with SR_',
    'missing raceDefName',
    'unspecified scope',
    'Race scope must not specify targetDefName',
    'Xenotype scope requires targetDefName',
    'weight must be finite and greater than zero',
    'duplicate fallback action',
    'duplicate action',
    'has no sounds',
    'without SR_ prefix',
    'sustained SoundDef',
    'context other than MapOnly',
    'has no SubSounds',
    'onCamera SubSound',
    'SubSound without grains',
    'null grain'
)
foreach ($marker in $validatorMarkers) {
    Assert-True ($validatorText.Contains($marker)) "Validator source is missing rule marker: $marker"
}

# 彩蛋路由闸（kernel 侧）也必须可定位，防止 XML/日志宣称 IsEgg 而内核不再执行过滤。
$kernelPath = Join-Path $root 'Source\SqueakyRatkin\Kernel\SqueakPoolRegistry.cs'
$kernelText = [System.IO.File]::ReadAllText($kernelPath)
Assert-True ($kernelText.Contains('set.IsEgg && !ctx.AllowEggs')) 'Kernel egg gate marker missing: set.IsEgg && !ctx.AllowEggs'

# ---------------------------------------------------------------- author-guide markers
$guidePath = Join-Path $root '.github\skills\squeaky-voicepack-authoring\SKILL.md'
$guideText = [System.IO.File]::ReadAllText($guidePath)
$guideMarkers = @(
    '公开稳定的作者 ABI',
    '字段只增不改',
    'append-only',
    'fail-closed',
    'raceDefName',
    'targetDefName',
    '<weight>',
    '<fallbacks>',
    '<ageTag>',
    '<IsEgg>true</IsEgg>',
    '<sustain>false</sustain>',
    '<context>MapOnly</context>',
    'onCamera',
    'SR_',
    'scripts/new-voicepack.ps1'
)
foreach ($marker in $guideMarkers) {
    Assert-True ($guideText.Contains($marker)) "Author guide/SKILL is missing ABI marker: $marker"
}
foreach ($key in $expectedKeys) {
    $token = '`' + $key + '`'
    Assert-True ($guideText.Contains($token)) "Author guide/SKILL is missing action key: $key"
}

# ---------------------------------------------------------------- verdict
if ($failures.Count -gt 0) {
    Write-Host '[FAIL] VoicePack XML ABI consistency lock:'
    foreach ($failure in $failures) { Write-Host "  - $failure" }
    exit 1
}

$eggNote = if ($eggTestAbsent) { "absent (dist artifact, skipped)" } else { "present" }
Write-Host "[OK] VoicePack XML ABI lock: $($xmlFiles.Count) XML samples x validator x author guide consistent (17 action keys; egg test pack $eggNote)."
