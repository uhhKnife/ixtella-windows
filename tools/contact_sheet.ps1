# Tile tour screenshots into labelled contact sheets (cols x rows per sheet).
#   contact_sheet.ps1 -Dir <folder with PNGs> [-Cols 4] [-Rows 2] [-Scale 0.5]
param([string]$Dir, [int]$Cols = 4, [int]$Rows = 2, [double]$Scale = 0.5)
Add-Type -AssemblyName System.Drawing
$files = Get-ChildItem $Dir -Filter "*.png" | Where-Object { $_.Name -notlike "sheet_*" } | Sort-Object Name
if ($files.Count -eq 0) { return }
$first = [System.Drawing.Image]::FromFile($files[0].FullName)
$w = [int]($first.Width * $Scale); $h = [int]($first.Height * $Scale); $first.Dispose()
$labelH = 18
$per = $Cols * $Rows
$font = New-Object System.Drawing.Font("Segoe UI", 9)
for ($s = 0; $s * $per -lt $files.Count; $s++) {
  $sheet = New-Object System.Drawing.Bitmap ($w * $Cols), (($h + $labelH) * $Rows)
  $g = [System.Drawing.Graphics]::FromImage($sheet)
  $g.Clear([System.Drawing.Color]::Black)
  $g.InterpolationMode = [System.Drawing.Drawing2D.InterpolationMode]::HighQualityBicubic
  for ($i = 0; $i -lt $per; $i++) {
    $k = $s * $per + $i
    if ($k -ge $files.Count) { break }
    $img = [System.Drawing.Image]::FromFile($files[$k].FullName)
    $x = ($i % $Cols) * $w; $y = [math]::Floor($i / $Cols) * ($h + $labelH)
    $g.DrawImage($img, $x, $y + $labelH, $w, $h)
    $g.DrawString($files[$k].BaseName, $font, [System.Drawing.Brushes]::White, $x + 2, $y + 1)
    $img.Dispose()
  }
  $g.Dispose()
  $out = Join-Path $Dir ("sheet_{0:00}.png" -f ($s + 1))
  $sheet.Save($out, [System.Drawing.Imaging.ImageFormat]::Png); $sheet.Dispose()
  $out
}
