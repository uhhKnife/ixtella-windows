# Capture a window to PNG without focusing it or moving the mouse (PrintWindow),
# and optionally send it a left click at client coordinates via window messages.
#   wincap.ps1 -Title <substring> -Out <png> [-ClickX x -ClickY y]
param([string]$ProcessName = "IkustellaLink", [string]$Out, [int]$ClickX = -1, [int]$ClickY = -1, [int]$DragToX = -1, [int]$DragToY = -1, [int]$Key = -1)
Add-Type -AssemblyName System.Drawing
Add-Type @"
using System; using System.Runtime.InteropServices;
public static class W {
  [StructLayout(LayoutKind.Sequential)] public struct RECT { public int L, T, R, B; }
  [DllImport("user32.dll")] public static extern bool GetClientRect(IntPtr h, out RECT r);
  [DllImport("user32.dll")] public static extern bool PrintWindow(IntPtr h, IntPtr hdc, uint f);
  [DllImport("user32.dll")] public static extern IntPtr PostMessage(IntPtr h, uint m, IntPtr w, IntPtr l);
}
"@
$p = Get-Process -Name $ProcessName -ErrorAction Stop | Select-Object -First 1
$h = $p.MainWindowHandle
if ($h -eq [IntPtr]::Zero) { throw "no window" }
function Get-LParam($x, $y) { return [IntPtr](($y -shl 16) -bor ($x -band 0xFFFF)) }
if ($ClickX -ge 0) {
  [W]::PostMessage($h, 0x0200, [IntPtr]0, (Get-LParam $ClickX $ClickY)) | Out-Null   # WM_MOUSEMOVE
  Start-Sleep -Milliseconds 80
  [W]::PostMessage($h, 0x0201, [IntPtr]1, (Get-LParam $ClickX $ClickY)) | Out-Null   # WM_LBUTTONDOWN
  Start-Sleep -Milliseconds 120
  if ($DragToX -ge 0) {
    for ($i = 1; $i -le 10; $i++) {
      $x = [int]($ClickX + ($DragToX - $ClickX) * $i / 10); $y = [int]($ClickY + ($DragToY - $ClickY) * $i / 10)
      [W]::PostMessage($h, 0x0200, [IntPtr]1, (Get-LParam $x $y)) | Out-Null; Start-Sleep -Milliseconds 30
    }
    [W]::PostMessage($h, 0x0202, [IntPtr]0, (Get-LParam $DragToX $DragToY)) | Out-Null
  } else {
    [W]::PostMessage($h, 0x0202, [IntPtr]0, (Get-LParam $ClickX $ClickY)) | Out-Null # WM_LBUTTONUP
  }
  Start-Sleep -Milliseconds 300
}
if ($Key -ge 0) {
  [W]::PostMessage($h, 0x0100, [IntPtr]$Key, [IntPtr]0) | Out-Null; Start-Sleep -Milliseconds 100
  [W]::PostMessage($h, 0x0101, [IntPtr]$Key, [IntPtr]0) | Out-Null; Start-Sleep -Milliseconds 300
}
if ($Out) {
  $r = New-Object W+RECT; [W]::GetClientRect($h, [ref]$r) | Out-Null
  $bmp = New-Object System.Drawing.Bitmap ($r.R - $r.L), ($r.B - $r.T)
  $g = [System.Drawing.Graphics]::FromImage($bmp); $hdc = $g.GetHdc()
  [W]::PrintWindow($h, $hdc, 3) | Out-Null   # PW_CLIENTONLY | PW_RENDERFULLCONTENT
  $g.ReleaseHdc($hdc); $g.Dispose(); $bmp.Save($Out, [System.Drawing.Imaging.ImageFormat]::Png); $bmp.Dispose()
  "captured $($r.R)x$($r.B) -> $Out"
}
