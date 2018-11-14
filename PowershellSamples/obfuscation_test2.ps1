function reorder_string($str1, $str2){
    return $str2+$str1
}

&(reorder_string -str1 "-object" -str2 "new") net.webclient

$script = "new-object system.net.webclient; $wc.downloadstring('C:\Users\Zhenyuan Li\Documents\GitHub\Invoke-Obfuscation\LICENSE')"

$script = "function reorder_string($str1, $str2){ return $str2+$str1 }; &(reorder_string -str1 '-object' -str2 'new') net.webclient"

$script = "&('Ne'+'w-'+'obje'+'ct') system.net.webclient; $wc.downloadstring((('C:c'+('{1}{0}' -f 'Users','rf')+'crf'+'Zh'+('{1}{0}' -f'ua','eny')+('{0}{1}' -f'n L','i')+'c'+('{1}{0}'-f'Docu','rf')+('{2}{0}{1}'-f 'tscr','f','men')+'G'+'i'+('{2}{5}{0}{1}{4}{3}' -f'fInvoke-Obfu','sc','t','ionc','at','Hubcr')+('{0}{2}{1}'-f'rfL','E','ICENS')) -rEPlACe'crf',[ChaR]92))"

# AST analysis
$AbstractsyntaxTree = [System.Management.Automation.Language.Parser]::ParseInput($script, [ref]$null, [ref]$null)
$AbstractsyntaxTree.FindAll({$args[0] -is [System.Management.Automation.Language.VariableExpressionAst]},$true)
$AbstractsyntaxTree.FindAll({$args[0] -is [System.Management.Automation.Language.CommandAst]}, $true) |foreach {
    $Command = $_.CommandElements[0]
    if ($Alias = Get-Alias | where { $_.Name -eq $Command }) {
        [PSCustomObject]@{
            Alias = $Alias.Name
            Definition = $Alias.Definition
            Start = $Command.Extent.StartOffset
            End = $Command.Extent.EndOffset
        }
    }   
} | Format-Table -AutoSize

$Token = [System.Management.Automation.PSParser]::Tokenize($script,[ref]$null)
$Token  | ForEach-Object {
    if ($_.Type -eq 'Command') {
        $Content = $_.Content
        if ($Alias = Get-Alias | Where-Object { $_.Name -eq $Content }) {
            New-Object PSObject -Property @{
                Alias = $Alias.Name
                Definition = $Alias.Definition
                Start = $_.Start
                Length = $_.Length
            }
        }
    }
} | Format-Table -AutoSize Alias, Definition, Start, Length