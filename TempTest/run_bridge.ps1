$exepath = "E:\steam\steamapps\common\Hollow Knight\BepInEx\plugins\HollowKnightHaptics\HapticsBridge\HapticsBridge.exe"
$logfile = "E:\steam\steamapps\common\Hollow Knight\BepInEx\plugins\HollowKnightHaptics\HapticsBridge\HapticsBridge.log"
$proc = Start-Process $exepath -PassThru -RedirectStandardOutput "$env:TEMP\bridge_out.txt" -RedirectStandardError "$env:TEMP\bridge_err.txt" -NoNewWindow
Start-Sleep -Seconds 8
Stop-Process $proc -Force -ErrorAction SilentlyContinue
Get-Content $logfile
Write-Host "--- stdout ---"
Get-Content "$env:TEMP\bridge_out.txt"
Write-Host "--- stderr ---"
Get-Content "$env:TEMP\bridge_err.txt"
