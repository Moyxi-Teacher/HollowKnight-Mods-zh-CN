@echo off
copy /Y "D:\KDQSHH\字体资产工作\LXGW-as-NotoSerifCJKSC.ttf" "C:\Windows\Fonts\LXGW-as-NotoSerifCJKSC.ttf" > "D:\KDQSHH\字体资产工作\install_log.txt" 2>&1
echo exit=%ERRORLEVEL% >> "D:\KDQSHH\字体资产工作\install_log.txt"
reg add "HKLM\SOFTWARE\Microsoft\Windows NT\CurrentVersion\Fonts" /v "Noto Serif CJK SC (TrueType)" /t REG_SZ /d "LXGW-as-NotoSerifCJKSC.ttf" /f >> "D:\KDQSHH\字体资产工作\install_log.txt" 2>&1
dir "C:\Windows\Fonts\LXGW*" >> "D:\KDQSHH\字体资产工作\install_log.txt" 2>&1
