# 将霞鹜臻楷安装到「当前用户」字体库（不需要管理员权限，可随时删除）
$ErrorActionPreference = 'Stop'

$src  = 'D:\KDQSHH\ModsWork\SimplifiedChineseFont-HEAD\霞鹜臻楷.ttf'
$dir  = Join-Path $env:LOCALAPPDATA 'Microsoft\Windows\Fonts'
$dst  = Join-Path $dir 'LXGWZhenKai.ttf'

if (-not (Test-Path $dir)) { New-Item -ItemType Directory -Force -Path $dir | Out-Null }
Copy-Item -LiteralPath $src -Destination $dst -Force
Write-Output ("已复制字体文件: " + $dst)

$reg = 'HKCU:\Software\Microsoft\Windows NT\CurrentVersion\Fonts'
if (-not (Test-Path $reg)) { New-Item -Path $reg -Force | Out-Null }

New-ItemProperty -Path $reg -Name 'LXGW ZhenKai (TrueType)'  -Value $dst -PropertyType String -Force | Out-Null
New-ItemProperty -Path $reg -Name '霞鹜臻楷 (TrueType)'        -Value $dst -PropertyType String -Force | Out-Null
Write-Output "已写入注册表项: HKCU\Software\Microsoft\Windows NT\CurrentVersion\Fonts"

# 通知系统刷新字体表
Add-Type -Namespace Win32 -Name Gdi -MemberDefinition '[DllImport("gdi32.dll", CharSet=CharSet.Unicode)] public static extern int AddFontResourceW(string lpFileName);'
[Win32.Gdi]::AddFontResourceW($dst) | Out-Null
Add-Type -Namespace Win32 -Name User -MemberDefinition '[DllImport("user32.dll")] public static extern int SendMessage(int hWnd, int Msg, int wParam, int lParam);'
[Win32.User]::SendMessage(0xFFFF, 0x001D, 0, 0) | Out-Null

Get-ItemProperty -Path $reg | Select-Object -Property 'LXGW*', '霞鹜*' | Format-List
Write-Output "安装完成（删除方式：删除 $dst 与注册表中对应两项即可）"
