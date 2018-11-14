using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using System.Diagnostics;

using System.Management.Automation;
using System.Management.Automation.Language;
using System.Collections.ObjectModel;

using TraceEvent2.PowerShellInteract;

namespace PowerShellInteract
{
    class Program
    {
        static void Main(string[] args)
        {

            //PowershellInstance ps = new PowershellInstance();

            String code = @"
            param( $Test)

            Function Measure-Sb{

                [CmdletBinding()]
                    [OutputType([Microsoft.Windows.PowerShell.ScriptAnalyzer.Generic.DiagnosticRecord[]])]

                Param(
                    [Parameter(Mandatory = $true)]
                    [ValidateNotNullOrEmpty()]
                    [System.Management.Automation.Language.ScriptBlockAst]
                    # [System.Management.Automation.Language.Ast] #NOK
                    # [System.Management.Automation.Language.FunctionDefinitionAst] #OK
                    $ObJectAST
                )

                process { 

                [Microsoft.Windows.PowerShell.ScriptAnalyzer.Generic.DiagnosticRecord]@{
                ¡®Message¡¯  = 'Test ScriptBlockAst'
                ¡®Extent¡¯   = $ObjectAST.Extent
                ¡®RuleName¡¯ = $PSCmdlet.MyInvocation.InvocationName
                ¡®Severity¡¯ = ¡®Warning¡¯
                }
            }
            }

            Function Test1
            {
                param($p1)
                'Test 1'
            }

            Function Test2($p1)
            {
                'Test 2'
            }
            ";

            ASTParser ast = new ASTParser(code);
            ast.Parser();


#if DEBUG
            System.Diagnostics.Debugger.Break();
#endif
        }
    }
}
