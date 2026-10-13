[CmdletBinding()]
param()

Set-StrictMode -Version Latest
$ErrorActionPreference = 'Stop'

# Independent, synthetic regression checks for privacy-audit.ps1. All fixture
# data is generated under the OS temp directory; no personal values are used.
$repo = Split-Path -Parent $PSScriptRoot
$auditSource = Join-Path $PSScriptRoot 'privacy-audit.ps1'
if (-not (Test-Path -LiteralPath $auditSource -PathType Leaf)) { throw 'privacy-audit.ps1 is missing.' }

$tempRoot = [IO.Path]::GetFullPath([IO.Path]::GetTempPath())
$fixtureRoot = [IO.Path]::GetFullPath((Join-Path $tempRoot ('sr-release-gates-' + [guid]::NewGuid().ToString('N'))))
$fixtureScripts = Join-Path $fixtureRoot 'scripts'
$fixtureAudit = Join-Path $fixtureScripts 'privacy-audit.ps1'
$results = New-Object 'System.Collections.Generic.List[string]'

function Assert-Case([bool]$Condition, [string]$Name) {
    if (-not $Condition) { throw "Regression case failed: $Name" }
    $script:results.Add($Name)
}

function Add-PngChunk([IO.MemoryStream]$Stream, [string]$Type, [byte[]]$Data) {
    $length = [byte[]]@((($Data.Length -shr 24) -band 255), (($Data.Length -shr 16) -band 255), (($Data.Length -shr 8) -band 255), ($Data.Length -band 255))
    $Stream.Write($length, 0, 4)
    $typeBytes = [Text.Encoding]::ASCII.GetBytes($Type)
    $Stream.Write($typeBytes, 0, 4)
    if ($Data.Length) { $Stream.Write($Data, 0, $Data.Length) }
    $Stream.Write([byte[]]@(0, 0, 0, 0), 0, 4) # CRC is not part of this parser's contract.
}

function New-Png([object[]]$Chunks) {
    $stream = [IO.MemoryStream]::new()
    try {
        $stream.Write([byte[]]@(137,80,78,71,13,10,26,10), 0, 8)
        Add-PngChunk $stream 'IHDR' ([byte[]]@(0,0,0,1,0,0,0,1,8,2,0,0,0))
        foreach ($chunk in $Chunks) { Add-PngChunk $stream $chunk.Type $chunk.Data }
        Add-PngChunk $stream 'IEND' ([byte[]]@())
        return ,$stream.ToArray()
    }
    finally { $stream.Dispose() }
}

function Compress-Bytes([byte[]]$Bytes) {
    $output = [IO.MemoryStream]::new()
    $zlib = [IO.Compression.ZLibStream]::new($output, [IO.Compression.CompressionLevel]::Optimal, $true)
    try { $zlib.Write($Bytes, 0, $Bytes.Length) }
    finally { $zlib.Dispose() }
    try { return ,$output.ToArray() }
    finally { $output.Dispose() }
}

function Invoke-Audit([switch]$FullHistory) {
    $args = @('-NoProfile', '-NonInteractive', '-File', $fixtureAudit)
    if ($FullHistory) { $args += '-FullHistory' }
    $output = @(& pwsh @args 2>&1 | ForEach-Object { [string]$_ }) -join "`n"
    $code = $LASTEXITCODE
    return [pscustomobject]@{ ExitCode = $code; Output = $output }
}

function Write-Fixture([string]$Path, [byte[]]$Bytes) {
    $full = Join-Path $fixtureRoot $Path
    [IO.Directory]::CreateDirectory((Split-Path -Parent $full)) | Out-Null
    [IO.File]::WriteAllBytes($full, $Bytes)
}

function New-OggPage([byte[]]$Packet, [uint32]$Serial) {
    if ($Packet.Length -gt 255) { throw 'Synthetic OGG packet must fit one lacing segment.' }
    $page = [IO.MemoryStream]::new()
    try {
        $header = [byte[]]::new(27)
        [Array]::Copy([Text.Encoding]::ASCII.GetBytes('OggS'), $header, 4)
        $header[4] = 0; $header[5] = 0
        [Array]::Copy([BitConverter]::GetBytes($Serial), 0, $header, 14, 4)
        $header[26] = 1
        $page.Write($header, 0, $header.Length)
        $lace = [byte[]]@($Packet.Length)
        $page.Write($lace, 0, 1)
        $page.Write($Packet, 0, $Packet.Length)
        return ,$page.ToArray()
    }
    finally { $page.Dispose() }
}

try {
    [IO.Directory]::CreateDirectory($fixtureScripts) | Out-Null
    Copy-Item -LiteralPath $auditSource -Destination $fixtureAudit
    Set-Content -LiteralPath (Join-Path $fixtureScripts 'privacy-history-exceptions.json') -Encoding utf8 -Value '{"exceptions":[]}'

    $init = @(& git -C $fixtureRoot init -q -b main 2>&1)
    if ($LASTEXITCODE -ne 0) { throw 'Could not initialize the temporary Git fixture.' }
    & git -C $fixtureRoot config user.name 'Synthetic Audit Fixture' | Out-Null
    & git -C $fixtureRoot config user.email '12345+fixture@users.noreply.github.com' | Out-Null

    $needle = 'Z:' + [char]92 + 'AuditFixture' + [char]92 + 'SyntheticMarker'
    $plain = [Text.Encoding]::UTF8.GetBytes($needle)
    $idatPayload = [byte[]](@(0,17,33,49) + [Text.Encoding]::ASCII.GetBytes($needle) + @(64,80,96))
    $idatPng = New-Png @(@{ Type = 'IDAT'; Data = $idatPayload })
    Write-Fixture 'idat-random.png' $idatPng

    # Ordinary text after IDAT, plus both compressed PNG text variants.
    Write-Fixture 'after-idat.png' (New-Png @(
        @{ Type = 'IDAT'; Data = [byte[]]@(1,2,3) },
        @{ Type = 'tEXt'; Data = [Text.Encoding]::Latin1.GetBytes('Comment' + [char]0 + $needle) }
    ))
    $compressed = Compress-Bytes $plain
    Write-Fixture 'ztxt.png' (New-Png @(@{ Type = 'zTXt'; Data = [byte[]](@([Text.Encoding]::ASCII.GetBytes('Comment')) + @(0,0) + $compressed) }))
    $compressedItxtPng = New-Png @(@{ Type = 'iTXt'; Data = [byte[]](@([Text.Encoding]::ASCII.GetBytes('Comment')) + @(0,1,0,0,0) + $compressed) })
    Write-Fixture 'itxt.png' $compressedItxtPng
    Write-Fixture 'renamed-itxt.dll' $compressedItxtPng
    Write-Fixture 'oversize.png' (New-Png @(@{ Type = 'tEXt'; Data = [byte[]]::new(1MB + 1) }))
    Write-Fixture 'bad-compression.png' (New-Png @(@{ Type = 'zTXt'; Data = [byte[]](@([Text.Encoding]::ASCII.GetBytes('Comment')) + @(0,0,1,2,3,4,5,6)) }))

    # Vorbis comment packet is the only metadata region. Following audio payload
    # deliberately contains a marker and must not be interpreted as a comment.
    $vendor = [Text.Encoding]::ASCII.GetBytes('fixture')
    $comment = [Text.Encoding]::ASCII.GetBytes('COMMENT=' + $needle)
    $packet = [IO.MemoryStream]::new()
    $packet.Write([byte[]]@(3) + [Text.Encoding]::ASCII.GetBytes('vorbis'), 0, 7)
    $packet.Write([BitConverter]::GetBytes([uint32]$vendor.Length), 0, 4); $packet.Write($vendor, 0, $vendor.Length)
    $packet.Write([BitConverter]::GetBytes([uint32]1), 0, 4)
    $packet.Write([BitConverter]::GetBytes([uint32]$comment.Length), 0, 4); $packet.Write($comment, 0, $comment.Length)
    $commentPage = New-OggPage $packet.ToArray() 11
    $packet.Dispose()
    $audioPage = New-OggPage ([Text.Encoding]::ASCII.GetBytes($needle)) 11
    Write-Fixture 'comments.ogg' ([byte[]]($commentPage + $audioPage))

    $cleanComment = [Text.Encoding]::ASCII.GetBytes('COMMENT=clean')
    $packet = [IO.MemoryStream]::new()
    $packet.Write([byte[]]@(3) + [Text.Encoding]::ASCII.GetBytes('vorbis'), 0, 7)
    $packet.Write([BitConverter]::GetBytes([uint32]$vendor.Length), 0, 4); $packet.Write($vendor, 0, $vendor.Length)
    $packet.Write([BitConverter]::GetBytes([uint32]1), 0, 4)
    $packet.Write([BitConverter]::GetBytes([uint32]$cleanComment.Length), 0, 4); $packet.Write($cleanComment, 0, $cleanComment.Length)
    $cleanCommentPage = New-OggPage $packet.ToArray() 12
    $packet.Dispose()
    $pathInAudioPage = New-OggPage ([Text.Encoding]::ASCII.GetBytes($needle)) 12
    Write-Fixture 'clean-audio.ogg' ([byte[]]($cleanCommentPage + $pathInAudioPage))

    & git -C $fixtureRoot add -- scripts idat-random.png after-idat.png ztxt.png itxt.png renamed-itxt.dll oversize.png bad-compression.png comments.ogg clean-audio.ogg
    & git -C $fixtureRoot commit -q -m 'Synthetic audit fixture baseline'
    if ($LASTEXITCODE -ne 0) { throw 'Could not commit the synthetic fixture baseline.' }

    $current = Invoke-Audit -FullHistory
    Assert-Case ($current.Output.Contains('vector4 binary metadata personal-path : after-idat.png')) 'PNG tEXt after IDAT is scanned'
    Assert-Case ($current.Output.Contains('vector4 binary metadata personal-path : ztxt.png')) 'zTXt is decompressed and scanned'
    Assert-Case ($current.Output.Contains('vector4 binary metadata personal-path : itxt.png')) 'compressed iTXt is decompressed and scanned'
    Assert-Case ($current.Output.Contains('vector4 binary metadata personal-path : renamed-itxt.dll')) 'PNG content signature selects metadata parser after rename to DLL'
    Assert-Case (-not $current.Output.Contains('vector4 binary metadata personal-path : idat-random.png')) 'IDAT bytes are excluded from metadata scanning'
    Assert-Case ($current.Output.Contains('vector4 metadata parsing failed: oversize.png')) 'oversized PNG metadata fails closed'
    Assert-Case ($current.Output.Contains('vector4 metadata parsing failed: bad-compression.png')) 'malformed PNG compression fails closed'
    Assert-Case ($current.Output.Contains('vector4 binary metadata personal-path : comments.ogg')) 'OGG Vorbis comments are parsed'
    Assert-Case (-not $current.Output.Contains('vector4 metadata parsing failed: comments.ogg')) 'OGG audio payload is not parsed as a comment'
    Assert-Case (-not $current.Output.Contains('vector4 binary metadata personal-path : clean-audio.ogg')) 'clean OGG comment plus path only in audio payload is not flagged'

    # A deleted PNG is only visible in historical binary scanning. The default
    # run must label history skipped; FullHistory must still identify its blob.
    $historic = New-Png @(@{ Type = 'tEXt'; Data = [Text.Encoding]::Latin1.GetBytes('Comment' + [char]0 + $needle) })
    Write-Fixture 'deleted-history.png' $historic
    & git -C $fixtureRoot add deleted-history.png
    & git -C $fixtureRoot commit -q -m 'Synthetic historical PNG'
    Remove-Item -LiteralPath (Join-Path $fixtureRoot 'deleted-history.png')
    & git -C $fixtureRoot add -u
    & git -C $fixtureRoot commit -q -m 'Remove synthetic historical PNG'
    $default = Invoke-Audit
    Assert-Case ($default.Output.Contains('vector3 (historical blobs): SKIPPED')) 'default mode explicitly reports history skipped'
    $full = Invoke-Audit -FullHistory
    Assert-Case ($full.Output.Contains('vector4 binary metadata personal-path : deleted-history.png :')) 'FullHistory scans deleted PNG metadata'

    # Exercise the .NET UNC expression through a real annotated-tag message.
    $twoSlashes = [string][char]92 + [char]92
    $unc = $twoSlashes + 'fixture-host' + [char]92 + 'share'
    & git -C $fixtureRoot tag -a synthetic-unc -m ('fixture ' + $unc)
    if ($LASTEXITCODE -ne 0) { throw 'Could not create synthetic annotated tag.' }
    $uncRun = Invoke-Audit
    Assert-Case ($uncRun.Output.Contains('vector2 commit messages unc-path')) 'UNC .NET pattern executes without exception and flags the synthetic tag'

    Assert-Case ($current.ExitCode -ne 0) 'positive fixture findings produce a failing audit exit code'
    Assert-Case (-not $default.Output.Contains('deleted-history.png')) 'default mode does not claim deleted historical PNG was scanned'
    Assert-Case ($full.ExitCode -ne 0) 'FullHistory fails on historical PNG metadata'

    Write-Host ("PASS: {0} synthetic release-gate regression cases" -f $results.Count)
    foreach ($result in $results) { Write-Host ("  [ok] {0}" -f $result) }
    exit 0
}
finally {
    # Delete only the exact unique fixture created by this run, after proving
    # its canonical path is a direct child of the canonical OS temp directory.
    $resolvedFixture = [IO.Path]::GetFullPath($fixtureRoot)
    $resolvedTemp = [IO.Path]::GetFullPath($tempRoot).TrimEnd([IO.Path]::DirectorySeparatorChar, [IO.Path]::AltDirectorySeparatorChar) + [IO.Path]::DirectorySeparatorChar
    if ($resolvedFixture.StartsWith($resolvedTemp, [StringComparison]::OrdinalIgnoreCase) -and
        (Split-Path -Leaf $resolvedFixture) -match '^sr-release-gates-[0-9a-f]{32}$') {
        if (Test-Path -LiteralPath $resolvedFixture) { Remove-Item -LiteralPath $resolvedFixture -Recurse -Force }
    }
    else { Write-Warning 'Fixture cleanup skipped because the canonical path did not pass the temp-root ownership check.' }
}
