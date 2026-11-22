# Run the -porttour screenshot tour safely: snapshot the save (or its absence) first and put it
# back exactly afterwards, then make contact sheets.
#   run_tour.ps1 -Lang en|ja [-Out <dir>] [-Arch win-x64|win-x86]
param([string]$Lang = "en", [string]$Out = "", [string]$Arch = "win-x64")
$port = Split-Path $PSScriptRoot -Parent
if (-not $Out) { $Out = Join-Path $port "logs\tour-$Lang" }
$save = Join-Path $env:USERPROFILE "AppData\LocalLow\Marvelous Inc_\育ステラ\FixlSavedata.json"
$bak = Join-Path $port "logs\tour-save-snapshot.json"
$hadSave = Test-Path $save
if ($hadSave) { Copy-Item $save $bak -Force } elseif (Test-Path $bak) { Remove-Item $bak -Force }
try {
  Remove-Item $Out -Recurse -Force -ErrorAction SilentlyContinue
  New-Item -ItemType Directory -Force $Out | Out-Null
  $exe = Join-Path $port "build\$Arch\IkustellaLink.exe"
  $g = Start-Process -FilePath $exe -ArgumentList @('-porttour', '-porttour-lang', $Lang, '-porttour-out', "`"$Out`"", '-screen-fullscreen', '0', '-screen-width', '540', '-screen-height', '960', '-logFile', "`"$Out\player.log`"") -PassThru
  if (-not $g.WaitForExit(400000)) { $g.Kill() }
}
finally {
  if ($hadSave) { Copy-Item $bak $save -Force; "save restored" }
  elseif (Test-Path $save) { Remove-Item $save -Force; "no save before the tour: tour-created save removed" }
}
"shots: $((Get-ChildItem $Out -Filter *.png).Count)"
& (Join-Path $PSScriptRoot "contact_sheet.ps1") -Dir $Out | Out-Null
