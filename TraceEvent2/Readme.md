# Requirement
## Windows 7

1. WMF 4.0 (Windows6.1-KB2819745-x64-MultiPkg.msu)
2. WMF 5.1 (Win7AndW2K8R2-KB3191566-x64.msu)
3. .Net Framework 4.6.1 (NDP461-KB3102438-Web.exe)

## Windows 7 & 10

Enable PSScriptBlockLogging & PSScriptBlockInvocationLogging  
Run following command:

    Import-Module Enable-PSScriptBlockLogging.ps1
    Enable-PSScriptBlockLogging


# Usage:

## For Powershell

    --provider=Microsoft-Windows-PowerShell --mode=s -r