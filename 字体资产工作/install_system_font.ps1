# 把「霞鹜臻楷（改名为 Noto Serif CJK SC）」安装为系统字体
# 作用：Unity 的游戏字体回退名正好是 Noto Serif CJK SC —— 装上后，界面中文就会用霞鹜臻楷渲染
# 可逆：删除 C:\Windows\Fonts\LXGW-as-NotoSerifCJKSC.ttf 与该注册表项即可
$ErrorActionPreference = 'Stop'
$src = 'D:\KDQSHH\字体资产工作\LXGW-as-NotoSerifCJKSC.ttf'
$dst = 'C:\Windows\Fonts\LXGW-as-NotoSerifCJKSC.ttf'
$reg = 'HKLM:\SOFTWARE\Microsoft\Windows NT\CurrentVersion\Fonts'

if (-not (Test-Path $src)) { Write-Output "字体源文件不存在: $src"; exit 1 }

Copy-Item -LiteralPath $src -Destination $dst -Force
Write-Output "已复制字体: $dst"

New-ItemProperty -Path $reg -Name 'Noto Serif CJK SC (TrueType)' -Value 'LXGW-as-NotoSerifCJKSC.ttf' -PropertyType String -Force | Out-Null
Write-Output "已写入注册表: $reg"

try {
  Add-Type -Namespace Win32 -Name Gdi -MemberDefinition '[DllImport("gdi32.dll", CharSet=CharSet.Unicode)] public static extern int AddFontResourceW(string lpFileName);'
  [Win32.Gdi]::AddFontResourceW($dst) | Out-Null
  Add-Type -Namespace Win32 -Name User -MemberDefinition '[DllImport("user32.dll")] public static extern int SendMessage(int hWnd, int Msg, int wParam, int lParam);'
  [Win32.User]::SendMessage(0xFFFF, 0x001D, 0, 0) | Out-Null   # WM_FONTCHANGE
  Write-Output "已通知系统刷新字体表"
} catch {
  Write-Output ("刷新字体表时告警（非致命）: " + $_.Exception.Message)
}

$f = Get-Item $dst
Write-Output ("安装完成: " + $f.Name + "  " + $f.Length + " 字节")
Write-Output "卸载方式: 删除该字体文件并删除注册表中的 'Noto Serif CJK SC (TrueType)' 项"
