cmd.exe /c WUSA.EXE .\Win7AndW2K8R2-KB3191566-x64.msu /quiet /norestart
cmd.exe /c WUSA.EXE .\Windows6.1-KB2819745-x64-MultiPkg.msu /quiet /norestart
cmd.exe /c NDP461-KB3102436-x86-x64-AllOS-ENU.exe

# Import-Module .\Install-Application.ps1
# Import-Module .\Install-Update.ps1

# Install-Application -InstallFilePath .\NDP461-KB3102438-Web.exe -InstallerParameters "/S" -RegistryKey HKLM:\SOFTWARE\Wow6432Node\Microsoft\Windows\CurrentVersion\Uninstall\dotNet461 -RegistryName DisplayVersion -RegistryValue 4.6.1
# Install-Update -InstallFilePath .\Win7AndW2K8R2-KB3191566-x64.msu -KBID KB3191566
# Install-Update -InstallFilePath .\Windows6.1-KB2819745-x64-MultiPkg.msu -KBID KB2819745
