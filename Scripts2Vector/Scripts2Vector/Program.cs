using System;
using System.Collections.Generic;
using System.Linq;
using System.Management.Automation.Language;
using System.Text;
using System.Threading.Tasks;
using System.IO;

namespace Scripts2Vector
{
    class Program
    {
        public static TextWriter Out = Console.Out;
        public static FileStream fnfs = new FileStream("FileNameList.txt", FileMode.OpenOrCreate, FileAccess.ReadWrite);
        public static TextWriter fileNameList = new StreamWriter(fnfs);
        public static FileStream featurefs = new FileStream("Feature.txt", FileMode.OpenOrCreate, FileAccess.ReadWrite);
        public static TextWriter featureList = new StreamWriter(featurefs);

        static void Main(string[] args)
        {
            string scriptPath = "/home/wushang/powershelldetection/sampledata_obfuscation_collection/";
            int count = 0;
            foreach(string ob in new List<string>{"string", "tokenall", "encodinghex", "encodingsecstring"})
            {
                for(int i = 1; i <= 2500; i++)
                {
                    string filePath = String.Format("{0}{1}_{2}.ps1", scriptPath, i, ob);
                    // Out.WriteLine(filePath);
                    string feature = Script2Feature(filePath);
                    if(feature.Length  == 0){
                        Out.WriteLine(filePath);
                        continue;
                    }
                    featureList.WriteLine(feature);
                    featureList.Flush();
                    fileNameList.WriteLine("{0}.ps1",i);
                    fileNameList.Flush();
                    count ++;
                }
                Out.WriteLine(count);
            }

        }

        static string Script2Feature(string scriptPath)
        {
            string script;
            try
            {
                FileStream fs = new FileStream(scriptPath, FileMode.Open, FileAccess.Read);
                TextReader scriptIn = new StreamReader(fs);
                script = scriptIn.ReadToEnd();
            }
            catch(IOException e)
            {
                // Out.WriteLine(e.ToString());
                // Console.ReadKey();
                return "";
            }
            
            // script string -> astType List<string> -> vector short[] -> vector string
            ASTParser ast = new ASTParser(script);
            List<string> asttypeList = ast.Parser();
            string feature = AST2VectorString(AST2Vector(asttypeList));
            // Console.Out.WriteLine(feature);
            return feature;
        }

        static void CountAstType(List<string> asttypeList)
        {
            HashSet<string> asttypeSet = new HashSet<string>(asttypeList);
            int count = 0;
            foreach(string asttype in asttypeSet)
            {
                Out.WriteLine(asttype);
                count ++;
            }
            Out.WriteLine(count);
        }

        static string AST2VectorString(short[] vector)
        {
            StringBuilder sb = new StringBuilder();
            sb.Append(vector[0]);
            for(int i = 0; i < 70; i++)
            {
                sb.Append(",");
                sb.Append(vector[i]);
            }
            return sb.ToString();
        }

        static short[] AST2Vector(List<string> asttypeList)
        {
            short[] vector = new short[71];
            foreach(string asttype in asttypeList)
            {
                switch(asttype)
                {
                    case "System.Management.Automation.Language.ArrayExpressionAst": vector[0] ++; break;
                    case "System.Management.Automation.Language.ArrayLiteralAst": vector[1] ++; break;
                    case "System.Management.Automation.Language.AssignmentStatementAst": vector[2] ++; break;
                    case "System.Management.Automation.Language.AttributeAst": vector[3] ++; break;
                    case "System.Management.Automation.Language.AttributeBaseAst": vector[4] ++; break;
                    case "System.Management.Automation.Language.AttributedExpressionAst": vector[5] ++; break;
                    case "System.Management.Automation.Language.BaseCtorInvokeMemberExpressionAst": vector[6] ++; break;
                    case "System.Management.Automation.Language.BinaryExpressionAst": vector[7] ++; break;
                    case "System.Management.Automation.Language.BlockStatementAst": vector[8] ++; break;
                    case "System.Management.Automation.Language.BreakStatementAst": vector[9] ++; break;
                    case "System.Management.Automation.Language.CatchClauseAst": vector[10] ++; break;
                    case "System.Management.Automation.Language.CommandAst": vector[11] ++; break;
                    case "System.Management.Automation.Language.CommandBaseAst": vector[12] ++; break;
                    case "System.Management.Automation.Language.CommandElementAst": vector[13] ++; break;
                    case "System.Management.Automation.Language.CommandExpressionAst": vector[14] ++; break;
                    case "System.Management.Automation.Language.CommandParameterAst": vector[15] ++; break;
                    case "System.Management.Automation.Language.ConfigurationDefinitionAst": vector[16] ++; break;
                    case "System.Management.Automation.Language.ConstantExpressionAst": vector[17] ++; break;
                    case "System.Management.Automation.Language.ContinueStatementAst": vector[18] ++; break;
                    case "System.Management.Automation.Language.ConvertExpressionAst": vector[19] ++; break;
                    case "System.Management.Automation.Language.DataStatementAst": vector[20] ++; break;
                    case "System.Management.Automation.Language.DoUntilStatementAst": vector[21] ++; break;
                    case "System.Management.Automation.Language.DoWhileStatementAst": vector[22] ++; break;
                    case "System.Management.Automation.Language.DynamicKeywordStatementAst": vector[23] ++; break;
                    case "System.Management.Automation.Language.ErrorExpressionAst": vector[24] ++; break;
                    case "System.Management.Automation.Language.ErrorStatementAst": vector[25] ++; break;
                    case "System.Management.Automation.Language.ExitStatementAst": vector[26] ++; break;
                    case "System.Management.Automation.Language.ExpandableStringExpressionAst": vector[27] ++; break;
                    case "System.Management.Automation.Language.ExpressionAst": vector[28] ++; break;
                    case "System.Management.Automation.Language.FileRedirectionAst": vector[29] ++; break;
                    case "System.Management.Automation.Language.ForEachStatementAst": vector[30] ++; break;
                    case "System.Management.Automation.Language.FunctionDefinitionAst": vector[31] ++; break;
                    case "System.Management.Automation.Language.FunctionMemberAst": vector[32] ++; break;
                    case "System.Management.Automation.Language.HashtableAst": vector[33] ++; break;
                    case "System.Management.Automation.Language.IfStatementAst": vector[34] ++; break;
                    case "System.Management.Automation.Language.IndexExpressionAst": vector[35] ++; break;
                    case "System.Management.Automation.Language.InvokeMemberExpressionAst": vector[36] ++; break;
                    case "System.Management.Automation.Language.LabeledStatementAst": vector[37] ++; break;
                    case "System.Management.Automation.Language.LoopStatementAst": vector[38] ++; break;
                    case "System.Management.Automation.Language.MemberAst": vector[39] ++; break;
                    case "System.Management.Automation.Language.MemberExpressionAst": vector[40] ++; break;
                    case "System.Management.Automation.Language.MergingRedirectionAst": vector[41] ++; break;
                    case "System.Management.Automation.Language.NamedAttributeArgumentAst": vector[42] ++; break;
                    case "System.Management.Automation.Language.NamedBlockAst": vector[43] ++; break;
                    case "System.Management.Automation.Language.ParamBlockAst": vector[44] ++; break;
                    case "System.Management.Automation.Language.ParameterAst": vector[45] ++; break;
                    case "System.Management.Automation.Language.ParenExpressionAst": vector[46] ++; break;
                    case "System.Management.Automation.Language.PipelineAst": vector[47] ++; break;
                    case "System.Management.Automation.Language.PipelineBaseAst": vector[48] ++; break;
                    case "System.Management.Automation.Language.PropertyMemberAst": vector[49] ++; break;
                    case "System.Management.Automation.Language.RedirectionAst": vector[50] ++; break;
                    case "System.Management.Automation.Language.ReturnStatementAst": vector[51] ++; break;
                    case "System.Management.Automation.Language.ScriptBlockAst": vector[52] ++; break;
                    case "System.Management.Automation.Language.ScriptBlockExpressionAst": vector[53] ++; break;
                    case "System.Management.Automation.Language.StatementAst": vector[54] ++; break;
                    case "System.Management.Automation.Language.StatementBlockAst": vector[55] ++; break;
                    case "System.Management.Automation.Language.StringConstantExpressionAst": vector[56] ++; break;
                    case "System.Management.Automation.Language.SubExpressionAst": vector[57] ++; break;
                    case "System.Management.Automation.Language.SwitchStatementAst": vector[58] ++; break;
                    case "System.Management.Automation.Language.ThrowStatementAst": vector[59] ++; break;
                    case "System.Management.Automation.Language.TrapStatementAst": vector[60] ++; break;
                    case "System.Management.Automation.Language.TryStatementAst": vector[61] ++; break;
                    case "System.Management.Automation.Language.TypeConstraintAst": vector[62] ++; break;
                    case "System.Management.Automation.Language.TypeDefinitionAst": vector[63] ++; break;
                    case "System.Management.Automation.Language.TypeExpressionAst": vector[64] ++; break;
                    case "System.Management.Automation.Language.UnaryExpressionAst": vector[65] ++; break;
                    case "System.Management.Automation.Language.UsingExpressionAst": vector[66] ++; break;
                    case "System.Management.Automation.Language.UsingStatementAst": vector[67] ++; break;
                    case "System.Management.Automation.Language.VariableExpressionAst": vector[68] ++; break;
                    case "System.Management.Automation.Language.WhileStatementAst": vector[69] ++; break;
                    case "System.Management.Automation.Language.ForStatementAst": vector[70] ++; break;
                    default: Console.Out.WriteLine(asttype); break;
                }
            }
            return vector;
        }
    }

    public class ASTParser // accessible to other project
    {
        String sampleScript;

        private List<Ast> commandAstList = new List<Ast>();

        public ASTParser()
        {

        }
        public ASTParser(String sampleScript)
        {
            this.sampleScript = sampleScript;
        }

        public List<string> Parser()
        {
            ScriptBlockAst sb = System.Management.Automation.Language.Parser.ParseInput(sampleScript, out Token[] tokens, out ParseError[] errors);

            // AST type list https://docs.microsoft.com/en-us/dotnet/api/system.management.automation.language?view=powershellsdk-1.1.0
            IEnumerable<Ast> astnodeList = sb.FindAll(delegate (Ast t) 
            //{ return t is System.Management.Automation.Language.CommandAst; }
            {return true; }
            , true);

            List<string> asttypeList = new List<string>();
            foreach(var astnode in astnodeList)
            {
                // Console.Out.WriteLine(astnode.GetType().ToString());
                asttypeList.Add(astnode.GetType().ToString());
                // Console.Out.WriteLine(astnode.ToString());
            }
            return asttypeList;
            //var funcAst = sb.FindAll(delegate (Ast t) { return t is System.Management.Automation.Language.FunctionDefinitionAst; }, true);

            // FindAllVisitor treeWalker = new FindAllVisitor();
            // List<Ast> command = treeWalker.Visit(sb);
        }


        public void Parser(String sampleScript)
        {
            this.sampleScript = sampleScript;
            //return Parser();
        }
    }

    class FindAllVisitor : AstVisitor
    {
        private List<Ast> commandAstList = new List<Ast>();

        void Initialize(Ast ast)
        {

        }

        public List<Ast> Visit(Ast ast)
        {
            if (!(ast is ScriptBlockAst || ast is FunctionMemberAst || ast is FunctionDefinitionAst))
            {

            }

            var visitor = new FindAllVisitor();

             if (ast is ScriptBlockAst)
                (ast as ScriptBlockAst).Visit(visitor);
            else if (ast is FunctionDefinitionAst)
                (ast as FunctionDefinitionAst).Body.Visit(visitor);
            else if (ast is FunctionMemberAst && (ast as FunctionMemberAst).Parameters != null)
                visitor.VisitParameters((ast as FunctionDefinitionAst).Parameters);

            List<string> commandList = new List<string>();
            foreach(var command in visitor.commandAstList)
            {
                string commandString = command.Extent.ToString();
                commandString = commandString.ToLower();
                commandList.Add(commandString);
                Console.Out.WriteLine(commandString);
            }

            return commandAstList;
        }

        internal void VisitParameters(IReadOnlyCollection<ParameterAst> parameters)
        {
            foreach (var t in parameters)
            {
                var variableExpressionAst = t.Name;
                var varPath = variableExpressionAst.VariablePath;
            }
        }

        public override AstVisitAction VisitScriptBlock(ScriptBlockAst scriptBlockAst)
        {
            return AstVisitAction.Continue;
        }

        public override AstVisitAction VisitFunctionDefinition(FunctionDefinitionAst functionDefinitionAst)
        {
            return AstVisitAction.Continue;
        }

        public override AstVisitAction VisitCommand(CommandAst commandAst)
        {
            commandAstList.Add(commandAst);
            return AstVisitAction.Continue;
        }
    }
}
