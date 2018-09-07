wevtutil cl "Microsoft-Windows-Powershell/Operational"
Get-WinEvent "Microsoft-Windows-Powershell/Operational" -Oldest | Where-Object Id -eq 4104 | ForEach-Object Message | Set-Clipboard