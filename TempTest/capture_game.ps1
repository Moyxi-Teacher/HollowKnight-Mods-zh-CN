# 给「空洞骑士」游戏窗口截屏（仅读取屏幕，不修改游戏任何文件）
Add-Type -AssemblyName System.Drawing
Add-Type @"
using System;
using System.Runtime.InteropServices;
public class Win32Cap {
    [DllImport("user32.dll", CharSet=CharSet.Unicode)] public static extern IntPtr FindWindow(string cls, string name);
    [DllImport("user32.dll")] public static extern bool GetWindowRect(IntPtr h, out RECT r);
    [DllImport("user32.dll")] public static extern bool SetForegroundWindow(IntPtr h);
    [DllImport("user32.dll")] public static extern bool ShowWindow(IntPtr h, int cmd);
    public struct RECT { public int Left; public int Top; public int Right; public int Bottom; }
}
"@
$h = [Win32Cap]::FindWindow($null, "Hollow Knight")
if ($h -eq [IntPtr]::Zero) {
  # 退而求其次：按进程名找主窗口
  $p = Get-Process -Name "hollow_knight" -ErrorAction SilentlyContinue | Where-Object { $_.MainWindowHandle -ne 0 } | Select-Object -First 1
  if ($p) { $h = $p.MainWindowHandle }
}
if ($h -eq [IntPtr]::Zero) { Write-Output "未找到游戏窗口（游戏没运行？）"; exit 1 }

[Win32Cap]::ShowWindow($h, 9) | Out-Null      # SW_RESTORE
[Win32Cap]::SetForegroundWindow($h) | Out-Null
Start-Sleep -Milliseconds 700

$r = New-Object Win32Cap+RECT
[Win32Cap]::GetWindowRect($h, [ref]$r) | Out-Null
$w = $r.Right - $r.Left; $ht = $r.Bottom - $r.Top
Write-Output ("窗口区域: {0},{1} {2}x{3}" -f $r.Left, $r.Top, $w, $ht)
$bmp = New-Object System.Drawing.Bitmap($w, $ht)
$g = [System.Drawing.Graphics]::FromImage($bmp)
$g.CopyFromScreen($r.Left, $r.Top, 0, 0, $bmp.Size)
$out = "D:\KDQSHH\TempTest\游戏截屏.png"
$bmp.Save($out, [System.Drawing.Imaging.ImageFormat]::Png)
$g.Dispose(); $bmp.Dispose()
Write-Output ("已保存: " + $out)
