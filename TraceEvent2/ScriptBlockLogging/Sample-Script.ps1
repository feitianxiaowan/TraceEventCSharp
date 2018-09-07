$time = (Get-Date).AddDays(-60)
$size = 1MB
$filepick = Get-ChildItem -Recurse | `
  Where-Object {$_.LastWriteTime -lt $time -and $_.Length -gt $size}

powershell -encodedCommand JAB0AGkAbQBlACAAPQAgACgARwBlAHQALQBEAGEAdABlACkALgBBAGQAZABEAGEAeQBzACgALQA2ADAAKQANAAoAJABzAGkAegBlACAAPQAgADEATQBCAA0ACgAkAGYAaQBsAGUAcABpAGMAawAgAD0AIABHAGUAdAAtAEMAaABpAGwAZABJAHQAZQBtACAALQBSAGUAYwB1AHIAcwBlACAAfAAgAGAADQAKACAAIABXAGgAZQByAGUALQBPAGIAagBlAGMAdAAgAHsAJABfAC4ATABhAHMAdABXAHIAaQB0AGUAVABpAG0AZQAgAC0AbAB0ACAAJAB0AGkAbQBlACAALQBhAG4AZAAgACQAXwAuAEwAZQBuAGcAdABoACAALQBnAHQAIAAkAHMAaQB6AGUAfQA=

# $command = 'dir'
# $bytes=[Text.Encoding]::Unicode.GetBytes($command)
# $encCmd=[Convert]::ToBase64String($bytes)
# powershell -encodedCommand $encCmd 

