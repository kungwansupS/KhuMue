<#
  Dumps the live registries (items, recipes, drops, shops, shimmer, NPCs, Boss Checklist
  entries, bestiary spawn tags) of a mod profile to JSON, using the tool-only BGDataDump mod.

  The run uses an isolated tModLoader profile, so the player's real Mods folder, worlds,
  characters and configs are never touched.

  Example:
    ./tools/RunDataDump.ps1 -TML <tModLoader installation> -Profile full -Out tools/data/dump
#>
param(
    [ValidateSet('vanilla', 'calamity', 'full')][string]$Profile = 'full',
    [Parameter(Mandatory = $true)][string]$Out,
    [string]$Work = (Join-Path $env:TEMP 'KhuMueDataDump'),
    [Parameter(Mandatory = $true)][string]$TML,
    # tModLoader release stream (year.month); picks the same Workshop version folder the game loads.
    # Empty = the installed tModLoader's own stream.
    [string]$TmlStream = ''
)
$ErrorActionPreference = 'Stop'
$TML = (Resolve-Path -LiteralPath $TML).Path
if (!(Test-Path -LiteralPath (Join-Path $TML 'tMLMod.targets'))) { throw 'TML must contain tMLMod.targets.' }
$src = Join-Path $PSScriptRoot 'BGDataDump'
$profileDir = Join-Path $Work "profile-$Profile"
$mods = Join-Path $profileDir 'Mods'
New-Item -ItemType Directory -Force $mods | Out-Null
New-Item -ItemType Directory -Force $Out | Out-Null

$sets = @{
    vanilla  = @('BGDataDump')
    calamity = @('BGDataDump', 'BossChecklist', 'CalamityModMusic', 'CalamityMod')
    full     = @('BGDataDump', 'BossChecklist', 'CalamityModMusic', 'CalamityMod', 'CatalystMod', 'CalamityHunt',
                 'SubworldLibrary', 'Luminance', 'Daybreak', 'NoxusBoss', 'InfernumMode', 'CalamityFables',
                 'SpiritMusic', 'SpiritReforged')
}
$enabled = $sets[$Profile]
$utf8 = [Text.UTF8Encoding]::new($false)

# A -nosteam server does not scan the Workshop folder, so copy the exact .tmod files the game
# itself would select: the newest version folder that is not newer than the installed stream.
$workshop = [IO.Path]::GetFullPath([IO.Path]::Combine($TML, '..', '..', 'workshop', 'content', '1281930'))
if (!$TmlStream) {
    # ProductVersion looks like "1.4.4.9+2026.08.3.0|2026.08|stable|...".
    $TmlStream = ([Diagnostics.FileVersionInfo]::GetVersionInfo((Join-Path $TML 'tModLoader.dll')).ProductVersion -split '\|')[1]
}
$stream = [version]$TmlStream
foreach ($name in $enabled) {
    if ($name -eq 'BGDataDump') { continue }
    $best = Get-ChildItem $workshop -Directory | ForEach-Object {
        Get-ChildItem $_.FullName -Directory | Where-Object { Test-Path (Join-Path $_.FullName "$name.tmod") } |
            ForEach-Object { [pscustomobject]@{ Path = (Join-Path $_.FullName "$name.tmod"); Ver = [version]$_.Name } }
    } | Where-Object { $_.Ver -le $stream } | Sort-Object Ver -Descending | Select-Object -First 1
    if (!$best) { throw "Workshop copy of $name not found" }
    $dest = Join-Path $mods "$name.tmod"
    if (!(Test-Path $dest) -or (Get-Item $dest).Length -ne (Get-Item $best.Path).Length) {
        Copy-Item $best.Path $dest -Force
    }
    Write-Output ("{0} <- {1}" -f $name, $best.Path)
}
# tModLoader rewrites enabled.json when a listed mod is missing, so write it after copying.
[IO.File]::WriteAllText((Join-Path $mods 'enabled.json'), (ConvertTo-Json -InputObject $enabled), $utf8)

& dotnet build (Join-Path $src 'BGDataDump.csproj') -c Release "-p:TMLInstallPath=$TML" '-p:BuildMod=false' --nologo -v q
if ($LASTEXITCODE -ne 0) { throw 'BGDataDump compile failed' }

Push-Location $TML
try {
    & "$TML\dotnet\dotnet.exe" "$TML\tModLoader.dll" -server -build $src -eac (Join-Path $src 'bin\Release\net8.0\BGDataDump.dll') `
        -savedirectory $profileDir -tmlsavedirectory $profileDir -modpath $mods *> (Join-Path $Out '_package.log')
    if ($LASTEXITCODE -ne 0) { throw 'BGDataDump packaging failed (see _package.log)' }

    Remove-Item (Join-Path $Out '_done.txt') -ErrorAction SilentlyContinue
    $env:BG_DUMP_OUT = $Out
    $env:BG_DUMP_EXIT = '1'
    & "$TML\dotnet\dotnet.exe" "$TML\tModLoader.dll" -server -savedirectory $profileDir -tmlsavedirectory $profileDir `
        -modpath $mods -nosteam *> (Join-Path $Out '_server.log')
}
finally {
    Remove-Item Env:\BG_DUMP_OUT -ErrorAction SilentlyContinue
    Remove-Item Env:\BG_DUMP_EXIT -ErrorAction SilentlyContinue
    Pop-Location
}
if (!(Test-Path (Join-Path $Out '_done.txt'))) { throw 'Dump did not finish (see _server.log)' }
Get-Content (Join-Path $Out '_log.txt')
