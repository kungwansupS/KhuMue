<#
  Builds Khu Mue and packages it into KhuMue.tmod in -Out.
  Nothing is installed: the game's own Mods folder and saves are never used.

  Example:
    ./tools/Package.ps1 -TML <tModLoader installation> -Out .qa
#>
param(
    [Parameter(Mandatory = $true)][string]$Out,
    [Parameter(Mandatory = $true)][string]$TML
)
$ErrorActionPreference = 'Stop'
# Windows PowerShell 5.1 does not set $PSScriptRoot inside param() defaults.
$here = Split-Path -Parent $MyInvocation.MyCommand.Path
$root = Split-Path $here -Parent
$TML = (Resolve-Path -LiteralPath $TML).Path
if (!(Test-Path -LiteralPath (Join-Path $TML 'tMLMod.targets'))) { throw 'TML must contain tMLMod.targets.' }
New-Item -ItemType Directory -Force $Out | Out-Null
$Out = (Resolve-Path $Out).Path
# tModLoader needs a save directory even when it only packages; keep it out of the real one.
$scratch = Join-Path $env:TEMP ('KhuMuePackage-' + [guid]::NewGuid().ToString('N'))
New-Item -ItemType Directory -Force $scratch | Out-Null

$dotnet = 'dotnet'
& $dotnet build (Join-Path $root 'KhuMue.csproj') -c Release "-p:TMLInstallPath=$TML" '-p:BuildMod=false' --nologo -v q
if ($LASTEXITCODE -ne 0) { throw 'Compile failed' }

$tmod = Join-Path $Out 'KhuMue.tmod'
# The packager always writes to <tmlsavedirectory>\Mods; the result is copied to -Out afterwards.
$built = Join-Path $scratch 'Mods\KhuMue.tmod'
Remove-Item $tmod, $built -ErrorAction SilentlyContinue
$log = Join-Path $Out '_package.log'
# tModLoader writes harmless warnings to stderr; Windows PowerShell would turn them into terminating
# errors under 'Stop', so the packager runs with 'Continue' and is judged by its exit code.
$saved = $ErrorActionPreference
$ErrorActionPreference = 'Continue'
Push-Location $TML
try {
    & "$TML\dotnet\dotnet.exe" "$TML\tModLoader.dll" -server -build $root `
        -eac (Join-Path $root 'bin\Release\net8.0\KhuMue.dll') `
        -savedirectory $scratch -tmlsavedirectory $scratch -modpath (Join-Path $scratch 'Mods') *> $log
    $code = $LASTEXITCODE
}
finally {
    Pop-Location
    $ErrorActionPreference = $saved
}
if ($code -ne 0 -or !(Test-Path $built)) { throw "Packaging failed (see $log)" }
Copy-Item $built $tmod -Force

$item = Get-Item $tmod
$hash = (Get-FileHash $tmod -Algorithm SHA256).Hash
"$($item.FullName)  $($item.Length) bytes  SHA256 $hash"
