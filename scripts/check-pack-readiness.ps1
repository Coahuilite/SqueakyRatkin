param(
    [string]$ProjectRoot = (Split-Path -Parent $PSScriptRoot),
    [switch]$SkipVerify,
    [switch]$RequireReleaseMetadata,
    [switch]$NoRestore
)

Set-StrictMode -Version Latest
$ErrorActionPreference = "Stop"

# 发布面单一入口（对齐 UniversalSqueaker 的 verify-local / check-pack-readiness / privacy-audit 三命令分工）：
#   - 日常开发门：scripts/verify-local.ps1（harness + 双 flavor 构建；本脚本默认会调用它）
#   - 发布面（本脚本）：版本轴、仓库红线、包内排除项、version.txt、DLL 身份，并把 Claim Pack 需要的字段
#     直接打印成 [claim] 行，发布时不再手工逐项核验与转抄
#   - 隐私面：scripts/privacy-audit.ps1（push 前唯一人工步骤）
# 包内容红线的写入方仍是 stage-package.ps1（写时断言），本脚本是发布前的读时复核——两者不是重复仪式：
# 写时保证"不会产出坏包"，读时保证"手上这个包确实是刚产出的那一个"。
#
# -SkipVerify              跳过 verify-local（CI 已单独跑过时用，避免重复构建）
# -RequireReleaseMetadata  发布口径：版本必须是无后缀基础版本、描述不得是占位符
# -NoRestore               转发给 verify-local：离线/已还原缓存环境（本机无网络时是必需项，否则 dotnet
#                          build 会尝试 restore 并以 NU1301 失败——这不是门本身的问题）
#
# 退出码 0 = 通过；1 = 存在失败项。

$root = [System.IO.Path]::GetFullPath($ProjectRoot)
$projectFile = Join-Path $root 'Source\SqueakyRatkin\SqueakyRatkin.csproj'
$aboutFile = Join-Path $root 'About\About.xml'
$versionedDir = Join-Path $root '1.6'
$assembliesDir = Join-Path $versionedDir 'Assemblies'
$templateAudioRoot = Join-Path $root 'Extras\SqueakyRatkinExampleVoices\1.6\Race\Sounds\coahuilite.squeakyratkin.examplevoices\SR_ExampleTemplate_Race'
$builtInSourceAudio = Join-Path $versionedDir 'Sounds\coahuilite.squeakyratkin\SR_OfficialExample_Race'
$failures = New-Object 'System.Collections.Generic.List[string]'

function Assert-Check {
    param([string]$Name, [bool]$Condition, [string]$Detail = '')
    if ($Condition) {
        Write-Host "[ok] $Name"
    }
    else {
        Write-Host "[FAIL] $Name $Detail"
        $script:failures.Add($Name)
    }
}

function Write-Note([string]$Text) {
    Write-Host "[note] $Text"
}

# ---- A. 版本轴与身份 ----
$projectXml = [xml](Get-Content -LiteralPath $projectFile -Raw)
$versionNode = $projectXml.SelectSingleNode('/Project/PropertyGroup/Version')
$version = if ($null -ne $versionNode) { $versionNode.InnerText.Trim() } else { '' }
Assert-Check 'csproj <Version> present' (-not [string]::IsNullOrWhiteSpace($version))

$aboutXml = [xml](Get-Content -LiteralPath $aboutFile -Raw)
$modVersionNode = $aboutXml.SelectSingleNode('/ModMetaData/modVersion')
$modVersion = if ($null -ne $modVersionNode) { $modVersionNode.InnerText.Trim() } else { '' }
Assert-Check 'About.xml <modVersion> present' (-not [string]::IsNullOrWhiteSpace($modVersion))
Assert-Check 'About.xml <modVersion> matches csproj <Version>' ($version -eq $modVersion) "($modVersion vs $version)"

$packageIdNode = $aboutXml.SelectSingleNode('/ModMetaData/packageId')
$packageId = if ($null -ne $packageIdNode) { $packageIdNode.InnerText.Trim() } else { '' }
Assert-Check 'packageId is coahuilite.squeakyratkin' ($packageId -eq 'coahuilite.squeakyratkin') "($packageId)"

Assert-Check 'LICENSE present at repo root and MPL-2.0' (
    (Test-Path -LiteralPath (Join-Path $root 'LICENSE') -PathType Leaf) -and
    ((Get-Content -LiteralPath (Join-Path $root 'LICENSE') -Raw) -match 'Mozilla Public License Version 2\.0')
)

if ($RequireReleaseMetadata) {
    $descNode = $aboutXml.SelectSingleNode('/ModMetaData/description')
    $desc = if ($null -ne $descNode) { $descNode.InnerText.Trim() } else { '' }
    Assert-Check 'release description is not a placeholder' ($desc -notmatch 'Placeholder|TODO') "($desc)"
    # 发布 tag 的基版本必须等于 csproj <Version>（release.yml 同断言）；带后缀的版本只允许存在于 tag 标签里。
    Assert-Check 'release <Version> is a bare MAJOR.MINOR.PATCH' ($version -match '^[0-9]+\.[0-9]+\.[0-9]+$') "($version)"
}

# ---- B. 加载与装配红线 ----
$loadFoldersPath = Join-Path $root 'LoadFolders.xml'
Assert-Check 'LoadFolders.xml exists' (Test-Path -LiteralPath $loadFoldersPath -PathType Leaf)
$loadFoldersText = Get-Content -LiteralPath $loadFoldersPath -Raw
# 0.2.1 教训：IfModActive 硬门控会让 fork 包名不匹配时整个模组静默不加载；XPath defName 匹配才是唯一入口。
Assert-Check 'LoadFolders.xml has no IfModActive gate' ($loadFoldersText -notmatch 'IfModActive')

Assert-Check 'no About/PublishedFileId.txt' (-not (Test-Path -LiteralPath (Join-Path $root 'About\PublishedFileId.txt') -PathType Leaf))

# Template 是 Example 音频的唯一维护源：内置镜像由 stage-package 在打包时生成，仓库里不得存在第二份源。
Assert-Check 'Template audio root exists' (Test-Path -LiteralPath $templateAudioRoot -PathType Container)
if (Test-Path -LiteralPath $templateAudioRoot -PathType Container) {
    $templateFiles = @(Get-ChildItem -LiteralPath $templateAudioRoot -Recurse -File)
    $nonOgg = @($templateFiles | Where-Object { $_.Extension -ne '.ogg' })
    Assert-Check 'Template audio root is non-empty' ($templateFiles.Count -gt 0)
    Assert-Check 'Template audio root is OGG-only' ($nonOgg.Count -eq 0) "(non-OGG: $($nonOgg.Count))"
    Write-Note "Template OGG clips: $($templateFiles.Count)"
}
Assert-Check 'no maintained built-in Example source in 1.6/Sounds' (-not (Test-Path -LiteralPath $builtInSourceAudio))

# ---- C. DLL 身份（不加载进本会话：读 Win32 版本资源） ----
$dllPath = Join-Path $assembliesDir 'SqueakyRatkin.dll'
$dllPresent = Test-Path -LiteralPath $dllPath -PathType Leaf
Assert-Check 'SqueakyRatkin.dll exists (build before a release check)' $dllPresent
if ($dllPresent) {
    $versionInfo = (Get-Item -LiteralPath $dllPath).VersionInfo
    $expectedFileVersion = $version + '.0'
    Assert-Check "DLL FileVersion is $expectedFileVersion" ($versionInfo.FileVersion -eq $expectedFileVersion) "($($versionInfo.FileVersion))"
    Assert-Check 'DLL ProductVersion contains the product version' ($versionInfo.ProductVersion -like ('*' + $version + '*')) "($($versionInfo.ProductVersion))"
}

# ---- D. 暂存包复核（存在才查；排除项与 version.txt 由 stage-package 写入） ----
$stageCandidates = @(
    @{ Flavor = 'dev'; Path = Join-Path $root 'dist\dev\SqueakyRatkin' },
    @{ Flavor = 'steam'; Path = Join-Path $root 'dist\steam\SqueakyRatkin' },
    @{ Flavor = 'github'; Path = Join-Path $root 'dist\github\SqueakyRatkin' }
)
$stagedSnapshots = New-Object 'System.Collections.Generic.List[string]'
foreach ($candidate in $stageCandidates) {
    $stagePath = $candidate.Path
    if (-not (Test-Path -LiteralPath $stagePath -PathType Container)) { continue }
    $flavor = $candidate.Flavor

    $stageAbout = Join-Path $stagePath 'About\About.xml'
    Assert-Check "[$flavor] staged About.xml present" (Test-Path -LiteralPath $stageAbout -PathType Leaf)
    $stageModVersion = ''
    if (Test-Path -LiteralPath $stageAbout -PathType Leaf) {
        $stageAboutXml = [xml](Get-Content -LiteralPath $stageAbout -Raw)
        $stageModVersionNode = $stageAboutXml.SelectSingleNode('/ModMetaData/modVersion')
        if ($null -ne $stageModVersionNode) { $stageModVersion = $stageModVersionNode.InnerText.Trim() }
    }

    $excludedHits = @(
        @(Get-ChildItem -LiteralPath $stagePath -Recurse -File -Filter '*.pdb')
        @(Get-ChildItem -LiteralPath $stagePath -Recurse -File -Filter '*.gitkeep')
        @(Get-ChildItem -LiteralPath $stagePath -Recurse -File -Filter 'codemap.md')
    )
    Assert-Check "[$flavor] staged package has no pdb/gitkeep/codemap" ($excludedHits.Count -eq 0) "(hits: $($excludedHits.Count))"
    Assert-Check "[$flavor] staged package has no PublishedFileId.txt" (-not (Test-Path -LiteralPath (Join-Path $stagePath 'About\PublishedFileId.txt') -PathType Leaf))

    $versionTxt = Join-Path $stagePath 'version.txt'
    Assert-Check "[$flavor] staged version.txt present" (Test-Path -LiteralPath $versionTxt -PathType Leaf)
    if (Test-Path -LiteralPath $versionTxt -PathType Leaf) {
        $lines = @(Get-Content -LiteralPath $versionTxt)
        # 标签可以带后缀（pre1/EXP 等）：stage-package 只断言"去掉后缀的基础版本 == About/About.xml modVersion"。
        $labelBase = if ($lines.Count -ge 1) { ($lines[0].Trim() -replace '^SqueakyRatkin ', '') -replace '-.*$', '' } else { '' }
        Assert-Check "[$flavor] version.txt line 1 base version is '$stageModVersion'" ($labelBase -eq $stageModVersion) "($($lines -join ' | '))"
        Assert-Check "[$flavor] version.txt line 2 is build=$flavor" ($lines.Count -ge 2 -and $lines[1].Trim() -eq ('build=' + $flavor)) ""
        Assert-Check "[$flavor] version.txt line 3 is commit=<sha>[-dirty]" ($lines.Count -ge 3 -and $lines[2].Trim() -match '^commit=[0-9a-f]{7,40}(-dirty)?$') ""
    }

    $stageFileCount = @(Get-ChildItem -LiteralPath $stagePath -Recurse -File).Count
    if ($stageModVersion -ne $version) {
        Write-Note "[$flavor] staged package is version $stageModVersion (csproj is $version): stale artifact, do not upload it"
    }
    # 一律输出仓库相对路径：这些行会被粘进 Claim Pack，绝不能携带本机展开路径。
    $relativeStagePath = $stagePath.Substring($root.Length).TrimStart('\', '/').Replace('\', '/')
    $entries = 'path=' + $relativeStagePath + '; files=' + $stageFileCount + '; version=' + $stageModVersion
    $stagedSnapshots.Add($entries)
    Write-Note "[$flavor] staged package: $entries"
}

# ---- E. 组合：默认跑一次本地自动校验 ----
if (-not $SkipVerify) {
    Write-Host '[run] verify-local.ps1 ...'
    $verifyArgs = @{ ProjectRoot = $root }
    if ($NoRestore) { $verifyArgs['NoRestore'] = $true }
    & (Join-Path $PSScriptRoot 'verify-local.ps1') @verifyArgs
    if ($LASTEXITCODE -ne 0) {
        $failures.Add('verify-local.ps1')
    }
}

if ($failures.Count -gt 0) {
    Write-Host ''
    Write-Host "[pack-readiness] FAIL: $($failures.Count) check(s) failed:"
    foreach ($failure in $failures) { Write-Host "  - $failure" }
    exit 1
}

# ---- Claim Pack 快照：发布时直接引用，不再手工逐项核验与转抄 ----
$infoValue = if ($dllPresent) { (Get-Item -LiteralPath $dllPath).VersionInfo.ProductVersion } else { '' }
$localHead = ''
try {
    $previous = $ErrorActionPreference
    $ErrorActionPreference = 'Continue'
    $headLines = @(& git -C $root rev-parse --short=12 HEAD 2>$null)
    if ($LASTEXITCODE -eq 0 -and $headLines.Count -gt 0) { $localHead = ([string]$headLines[0]).Trim() }
    $ErrorActionPreference = $previous
}
catch { }

Write-Host ''
Write-Host ('[claim] version=' + $version)
Write-Host ('[claim] packageId=' + $packageId)
Write-Host ('[claim] localHead=' + $(if ([string]::IsNullOrWhiteSpace($localHead)) { 'unknown' } else { $localHead }))
Write-Host ('[claim] dll.FileVersion=' + $(if ($dllPresent) { (Get-Item -LiteralPath $dllPath).VersionInfo.FileVersion } else { 'missing' }))
Write-Host ('[claim] dll.ProductVersion=' + $infoValue)
foreach ($snapshot in $stagedSnapshots) { Write-Host ('[claim] staged ' + $snapshot) }
Write-Host ''
Write-Host '[pack-readiness] all checks passed.'
