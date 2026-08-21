param(
    [string]$ProjectRoot = (Split-Path -Parent $PSScriptRoot),
    [switch]$PackDev,
    [switch]$PackSteam
)

Set-StrictMode -Version Latest
$ErrorActionPreference = "Stop"

# 一次输入本地自动校验（2026-08-20）。
# 顺序（fail-fast，任一失败即停并给出该检查的单独重跑命令）：
#   1-3  三 harness：内核纯度门 + 48 断言 + 已提交语料 3782 例字节回放；
#        设置 fixture 9 场景字节稳定；日志协议 v1 双 flavor（Release + Dev）。
#   4    fixtures/ 零 delta 门（语料与期望文件必须与提交内容逐字节一致）。
#   5-7   主模组 Dev flavor 构建 → Steam flavor 构建 → Dev 重建（恢复 Assemblies 为 Dev 态）；
#         TreatWarningsAsErrors=true 锁定 0 warning 基线。
# -PackDev   ：校验全绿后追加 dev 包（pack-dev，允许脏树并自动 -dirty 标签）。
# -PackSteam ：校验全绿后追加 Steam 包（build-steam = Steam flavor 构建 + pack-steam 干净树硬门；
#              发布前最终态，结束后 Assemblies 为 Steam 态）。
# 输出每检查一行 [OK]/[FAIL]，失败时仅回显末 12 行日志与重跑命令——把整会话验证收敛为一次输入。

$root = [System.IO.Path]::GetFullPath($ProjectRoot)
$projectFile = Join-Path $root 'Source\SqueakyRatkin\SqueakyRatkin.csproj'
$tempLog = Join-Path ([System.IO.Path]::GetTempPath()) ("sr-verify-" + [guid]::NewGuid().ToString('N') + '.log')

function Invoke-Check {
    param([string]$Name, [string]$Retry, [scriptblock]$Action)

    Write-Host -NoNewline "[run] $Name ... "
    & $Action *> $tempLog
    $code = $LASTEXITCODE
    if ($code -ne 0) {
        Write-Host 'FAIL'
        Get-Content -LiteralPath $tempLog -Tail 12 | ForEach-Object { Write-Host "    $_" }
        Write-Host "  retry: $Retry"
        Remove-Item -LiteralPath $tempLog -Force -ErrorAction SilentlyContinue
        exit 1
    }
    Write-Host 'OK'
}

Invoke-Check 'KernelCharacterization (purity gate, 48 asserts, committed corpus 3782 byte replay)' `
    'dotnet run --project tools/KernelCharacterization -c Release' `
    { dotnet run --project (Join-Path $root 'tools\KernelCharacterization') -c Release }

Invoke-Check 'SettingsFixtureGenerator (9 scenarios byte-stable)' `
    'dotnet run --project tools/SettingsFixtureGenerator -c Release' `
    { dotnet run --project (Join-Path $root 'tools\SettingsFixtureGenerator') -c Release }

Invoke-Check 'SqueakLogCharacterization Release (v1 protocol)' `
    'dotnet run --project tools/SqueakLogCharacterization -c Release' `
    { dotnet run --project (Join-Path $root 'tools\SqueakLogCharacterization') -c Release }

Invoke-Check 'SqueakLogCharacterization Dev (v1 protocol, SQUEAKY_DEV)' `
    'dotnet run --project tools/SqueakLogCharacterization -c Dev' `
    { dotnet run --project (Join-Path $root 'tools\SqueakLogCharacterization') -c Dev }

Invoke-Check 'fixtures zero-delta (corpus + expected byte-stable vs committed)' `
    'git -C . diff --exit-code -- fixtures/' `
    { git -C $root diff --exit-code -- fixtures/ }

Invoke-Check 'main mod Dev flavor build (warnings as errors)' `
    'dotnet build Source/SqueakyRatkin/SqueakyRatkin.csproj -c Release -p:TreatWarningsAsErrors=true' `
    { dotnet build $projectFile -c Release -p:TreatWarningsAsErrors=true }

Invoke-Check 'main mod Steam flavor build (warnings as errors)' `
    'dotnet build Source/SqueakyRatkin/SqueakyRatkin.csproj -c Release -p:SqueakyBuildFlavor=Steam -p:TreatWarningsAsErrors=true' `
    { dotnet build $projectFile -c Release -p:SqueakyBuildFlavor=Steam -p:TreatWarningsAsErrors=true }

Invoke-Check 'main mod Dev flavor rebuild (restore Assemblies to Dev state)' `
    'dotnet build Source/SqueakyRatkin/SqueakyRatkin.csproj -c Release -p:TreatWarningsAsErrors=true' `
    { dotnet build $projectFile -c Release -p:TreatWarningsAsErrors=true }

Write-Host '[verify] all checks passed.'

if ($PackDev) {
    Write-Host '[pack] dev package'
    & (Join-Path $PSScriptRoot 'pack-dev.ps1') -ProjectRoot $root
    if ($LASTEXITCODE -ne 0) { exit 1 }
}

if ($PackSteam) {
    Write-Host '[pack] steam package (Steam flavor build + clean-tree gate)'
    & (Join-Path $PSScriptRoot 'build-steam.ps1') -ProjectRoot $root
    if ($LASTEXITCODE -ne 0) { exit 1 }
    Write-Host '[pack] note: Assemblies are now Steam flavor. Run build-dev.ps1 before local dev work.'
}
