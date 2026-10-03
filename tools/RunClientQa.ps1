<#
  In-game QA for Khu Mue [Calamity] in an isolated tModLoader profile.

  Builds the guide and the test-only GuideQaProbe mod, packages both into a throw-away profile
  together with copies of the content mods, then starts the game client straight into a fixture
  world. The probe builds every page offscreen (exceptions, missing texts, raw placeholders) and saves
  screenshots of the pages listed in the plan. The player's real saves, configs and Mods folder are
  never used: every path points into -Work.

  -Multiplayer starts a dedicated server with the same mods and fixture world in its own profile and lets the
  client join it, so the guide is tested as a multiplayer client (tools/qa-plan-mp.json records the route state
  of every entry; compare it with a single-player run of the same plan).

  Example:
    ./tools/RunClientQa.ps1 -TML <tModLoader installation> -Fixtures <local fixtures> -Out .qa/client
#>
param(
    [Parameter(Mandatory = $true)][string]$Out,
    [ValidateSet('vanilla', 'full')][string]$Set = 'full',
    [string]$Plan = '',
    [string]$Work = (Join-Path $env:TEMP 'KhuMueClientQa'),
    [Parameter(Mandatory = $true)][string]$TML,
    # tModLoader release stream (year.month); empty = the installed tModLoader's own stream.
    [string]$TmlStream = '',
    # Fixture character and world (copies are made; the originals are only read).
    [string]$Fixtures = '',
    # Optional Thai pack to test the Thai texts and font (copied, never modified).
    [string]$ThaiPack = (Join-Path ([Environment]::GetFolderPath('MyDocuments')) 'My Games\Terraria\tModLoader\Mods\ThaiOfflineAllMods.tmod'),
    [int]$TimeoutSeconds = 900,
    # Extra Workshop mods for a compatibility check (e.g. BetterExpertRarity).
    [string[]]$Extra = @(),
    # Optional key bindings to test with, e.g. a copy of the player's "input profiles.json" (only read).
    [string]$InputProfile = '',
    # Interface scale of the test client (Settings > Interface > UI Scale); layouts and clicks depend on it.
    [double]$UIScale = 1.0,
    # Test with a player's own mod list (an enabled.json) instead of the built-in set. Each mod comes from the
    # Workshop or from -LocalMods, whichever has the higher version, as tModLoader picks (both are only read).
    [string]$ModList = '',
    [string]$LocalMods = '',
    [switch]$Multiplayer,
    [int]$Port = 7791
)
$ErrorActionPreference = 'Stop'
$TML = (Resolve-Path -LiteralPath $TML).Path
if (!(Test-Path -LiteralPath (Join-Path $TML 'tMLMod.targets'))) { throw 'TML must contain tMLMod.targets.' }
# Windows PowerShell 5.1 does not set $PSScriptRoot inside param() defaults.
$here = Split-Path -Parent $MyInvocation.MyCommand.Path
$root = Split-Path $here -Parent
if (!$Plan) { $Plan = Join-Path $here 'qa-plan.json' }
if (!$Fixtures) { $Fixtures = Join-Path $here 'qa-fixtures' }
$profileDir = Join-Path $Work $(if ($ModList) { 'profile-list' } else { "profile-$Set" })
$mods = Join-Path $profileDir 'Mods'
$qaDir = Join-Path $profileDir 'qa'
New-Item -ItemType Directory -Force $mods, $qaDir, $Out, (Join-Path $profileDir 'Players'), (Join-Path $profileDir 'Worlds') | Out-Null
$utf8 = [Text.UTF8Encoding]::new($false)

# ------------------------------------------------------------------ fixtures
foreach ($f in 'Players\AYO.plr', 'Players\AYO.tplr', 'Worlds\FF.wld', 'Worlds\FF.twld') {
    Copy-Item (Join-Path $Fixtures $f) (Join-Path $profileDir $f) -Force
}
if ($InputProfile) { Copy-Item $InputProfile (Join-Path $profileDir 'input profiles.json') -Force }
$config = Get-Content (Join-Path $Fixtures 'config.json') -Raw | ConvertFrom-Json
$config.Language = 'en-US'
$config.UIScale = $UIScale
[IO.File]::WriteAllText((Join-Path $profileDir 'config.json'), ($config | ConvertTo-Json -Depth 8), $utf8)

# ------------------------------------------------------------------ content mods
$sets = @{
    vanilla = @()
    full    = @('BossChecklist', 'CalamityModMusic', 'CalamityMod', 'CatalystMod', 'CalamityHunt', 'SubworldLibrary',
                'Luminance', 'Daybreak', 'NoxusBoss', 'InfernumMode', 'CalamityFables', 'SpiritMusic', 'SpiritReforged')
}
$workshop = [IO.Path]::GetFullPath([IO.Path]::Combine($TML, '..', '..', 'workshop', 'content', '1281930'))
if (!$TmlStream) {
    # ProductVersion looks like "1.4.4.9+2026.08.3.0|2026.08|stable|...".
    $TmlStream = ([Diagnostics.FileVersionInfo]::GetVersionInfo((Join-Path $TML 'tModLoader.dll')).ProductVersion -split '\|')[1]
}
$stream = [version]$TmlStream

# Mod version from a .tmod header ("TMOD", tML version, hash, signature, data length, name, version).
function Get-TmodVersion([string]$path) {
    $fs = [IO.File]::OpenRead($path)
    try {
        $br = [IO.BinaryReader]::new($fs)
        [void]$br.ReadBytes(4)
        [void]$br.ReadString()
        [void]$br.ReadBytes(20 + 256 + 4)
        [void]$br.ReadString()
        return [version]$br.ReadString()
    }
    finally { $fs.Dispose() }
}

$names = if ($ModList) {
    @((Get-Content $ModList -Raw | ConvertFrom-Json) | Where-Object { $_ -notin 'KhuMue', 'BetterGuide', 'GuideQaProbe', 'ThaiOfflineAllMods' })
} else { $sets[$Set] }
$names = @(@($names) + $Extra | Select-Object -Unique)
foreach ($name in $names) {
    $best = Get-ChildItem $workshop -Directory | ForEach-Object {
        Get-ChildItem $_.FullName -Directory | Where-Object { Test-Path (Join-Path $_.FullName "$name.tmod") } |
            ForEach-Object { [pscustomobject]@{ Path = (Join-Path $_.FullName "$name.tmod"); Ver = [version]$_.Name } }
    } | Where-Object { $_.Ver -le $stream } | Sort-Object Ver -Descending | Select-Object -First 1
    $local = if ($LocalMods) { Join-Path $LocalMods "$name.tmod" } else { '' }
    if ($local -and (Test-Path $local) -and (!$best -or (Get-TmodVersion $local) -ge (Get-TmodVersion $best.Path))) {
        $best = [pscustomobject]@{ Path = $local }
    }
    if (!$best) { throw "No copy of $name found (Workshop or -LocalMods)" }
    $dest = Join-Path $mods "$name.tmod"
    if (!(Test-Path $dest) -or (Get-Item $dest).Length -ne (Get-Item $best.Path).Length) { Copy-Item $best.Path $dest -Force }
}
$enabled = @('KhuMue', 'GuideQaProbe') + $names
if (Test-Path $ThaiPack) {
    Copy-Item $ThaiPack (Join-Path $mods 'ThaiOfflineAllMods.tmod') -Force
    $enabled += 'ThaiOfflineAllMods'
    # Start in English; the probe switches to Thai for the Thai screenshots.
    $cfgDir = Join-Path $profileDir 'ModConfigs'
    New-Item -ItemType Directory -Force $cfgDir | Out-Null
    [IO.File]::WriteAllText((Join-Path $cfgDir 'ThaiOfflineAllMods_ThaiConfig.json'), '{ "UseThai": false }', $utf8)
}

# ------------------------------------------------------------------ build and package
$dotnet = 'dotnet'
& $dotnet build (Join-Path $root 'KhuMue.csproj') -c Release "-p:TMLInstallPath=$TML" '-p:BuildMod=false' --nologo -v q
if ($LASTEXITCODE -ne 0) { throw 'KhuMue compile failed' }
& $dotnet build (Join-Path $root 'tools\GuideQaProbe\GuideQaProbe.csproj') -c Release "-p:TMLInstallPath=$TML" '-p:BuildMod=false' --nologo -v q
if ($LASTEXITCODE -ne 0) { throw 'GuideQaProbe compile failed' }

# tModLoader writes harmless warnings to stderr; Windows PowerShell would turn them into terminating
# errors under 'Stop', so native calls run with 'Continue' and are judged by their exit code.
function Invoke-Tml([string]$log, [string[]]$arguments) {
    $saved = $ErrorActionPreference
    $ErrorActionPreference = 'Continue'
    try { & "$TML\dotnet\dotnet.exe" "$TML\tModLoader.dll" @arguments *> $log }
    finally { $ErrorActionPreference = $saved }
    return $LASTEXITCODE
}

Push-Location $TML
try {
    $common = @('-savedirectory', $profileDir, '-tmlsavedirectory', $profileDir, '-modpath', $mods)
    $code = Invoke-Tml (Join-Path $Out '_package-guide.log') (@('-server', '-build', $root, '-eac', (Join-Path $root 'bin\Release\net8.0\KhuMue.dll')) + $common)
    if ($code -ne 0) { throw 'KhuMue packaging failed (see _package-guide.log)' }
    $probe = Join-Path $here 'GuideQaProbe'
    $code = Invoke-Tml (Join-Path $Out '_package-probe.log') (@('-server', '-build', $probe, '-eac', (Join-Path $probe 'bin\Release\net8.0\GuideQaProbe.dll')) + $common)
    if ($code -ne 0) { throw 'GuideQaProbe packaging failed (see _package-probe.log)' }

    # tModLoader rewrites enabled.json when a listed mod is missing, so write it after packaging.
    [IO.File]::WriteAllText((Join-Path $mods 'enabled.json'), (ConvertTo-Json -InputObject $enabled), $utf8)

    Copy-Item $Plan (Join-Path $qaDir 'plan.json') -Force
    Remove-Item (Join-Path $qaDir 'qa-report.json'), (Join-Path $qaDir 'shot-*.png') -ErrorAction SilentlyContinue
    $env:BG_QA_PLAN = Join-Path $qaDir 'plan.json'
    $start = @('-skipselect', '"AYO:FF"')
    if ($Multiplayer) {
        # The server gets its own profile (world, configs) but reads the same Mods folder; it skips
        # client-only mods such as the guide and the probe by itself.
        $serverDir = Join-Path $Work 'profile-server'
        New-Item -ItemType Directory -Force (Join-Path $serverDir 'Worlds') | Out-Null
        foreach ($f in 'FF.wld', 'FF.twld') { Copy-Item (Join-Path $Fixtures "Worlds\$f") (Join-Path $serverDir "Worlds\$f") -Force }
        $serverArgs = @('-server', '-savedirectory', $serverDir, '-tmlsavedirectory', $serverDir, '-modpath', $mods,
                        '-world', (Join-Path $serverDir 'Worlds\FF.wld'), '-port', "$Port", '-players', '4', '-noupnp')
        $server = Start-Process -FilePath "$TML\dotnet\dotnet.exe" -WorkingDirectory $TML -PassThru -WindowStyle Minimized `
            -ArgumentList (@("`"$TML\tModLoader.dll`"") + ($serverArgs | ForEach-Object { "`"$_`"" }))
        $deadline = (Get-Date).AddSeconds(600)
        $listening = $false
        while (!$listening -and (Get-Date) -lt $deadline -and !$server.HasExited) {
            Start-Sleep -Seconds 3
            try { $tcp = [Net.Sockets.TcpClient]::new('127.0.0.1', $Port); $tcp.Dispose(); $listening = $true } catch { }
        }
        if (!$listening) { throw 'The dedicated server did not start (see _server.log)' }
        $start = @('-join', "127.0.0.1:$Port", '-plr', 'AYO')
    }
    $client = Start-Process -FilePath "$TML\dotnet\dotnet.exe" -WorkingDirectory $TML -PassThru `
        -ArgumentList (@("`"$TML\tModLoader.dll`"") + ($common | ForEach-Object { "`"$_`"" }) + $start)
    if (!$client.WaitForExit($TimeoutSeconds * 1000)) {
        Stop-Process -Id $client.Id -Force
        throw "Client did not finish within $TimeoutSeconds s"
    }
}
finally {
    Remove-Item Env:\BG_QA_PLAN -ErrorAction SilentlyContinue
    if ($server -and !$server.HasExited) { Stop-Process -Id $server.Id -Force }
    Pop-Location
}

Copy-Item (Join-Path $qaDir '*') $Out -Force
# tModLoader keeps its logs next to the install, not in the save folder, and switches to client2.log,
# client3.log ... while another game instance holds client.log: take the newest.
$log = Get-ChildItem (Join-Path $TML 'tModLoader-Logs') -Filter 'client*.log' -ErrorAction SilentlyContinue |
    Sort-Object LastWriteTime -Descending | Select-Object -First 1
if ($log) { Copy-Item $log.FullName (Join-Path $Out '_client.log') -Force }
if ($Multiplayer) {
    $serverLog = Get-ChildItem (Join-Path $TML 'tModLoader-Logs') -Filter 'server*.log' -ErrorAction SilentlyContinue |
        Sort-Object LastWriteTime -Descending | Select-Object -First 1
    if ($serverLog) { Copy-Item $serverLog.FullName (Join-Path $Out '_server.log') -Force }
}
if (!(Test-Path (Join-Path $Out 'qa-report.json'))) { throw 'QA report missing (see _client.log)' }
Get-Content (Join-Path $Out 'qa-report.json')
