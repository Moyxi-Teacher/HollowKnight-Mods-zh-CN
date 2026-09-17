@echo off
chcp 65001 >nul
copy /Y "D:\KDQSHH\字体资产工作\LXGW-as-NotoSerifCJKSC.ttf" "%WINDIR%\Fonts\LXGW-as-NotoSerifCJKSC.ttf"
reg add "HKLM\SOFTWARE\Microsoft\Windows NT\CurrentVersion\Fonts" /v "Noto Serif CJK SC (TrueType)" /t REG_SZ /d "LXGW-as-NotoSerifCJKSC.ttf" /f
echo done
