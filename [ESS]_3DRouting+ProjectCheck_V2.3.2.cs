using System.Windows.Forms;

public class ESS_3DRouting_ProjectCheck_V2_3_2
{    
    [DeclareAction("ESS_3DRoutingAndCheck")]
    public void XESSRoutingAndCheck(string TYPE, string VERIFYSCHEME)
    {
        //DESCRIPTION:
        //Prüflauf aus Übergabeparameter 'VERIFYSCHEME' ausführen, und danach im Bauraum verlegen (Verlegart abhängig von Parameter 'TYPE') 
                
        //Anzeige der MV löschen       
        new CommandLineInterpreter().Execute("GfDlgMgrActionIGfWind /DialogName:MsgManagementDlg /function:Clear");

        switch (TYPE)
        {
            case "1":
                //Verbindungen: Verlegen im Bauraum    
                new CommandLineInterpreter().Execute("XCabSelect3DConnectionsToRouteAction");
                break;
            case "2":
                //Verbindungen: Freies Verlegen
                new CommandLineInterpreter().Execute("XCabSelect3DConnectionsToRouteWithoutNetworkAction");
                break;
            default:
                MessageBox.Show("Keine gültige Verlegeart im Parameter /TYPE vorhanden !\nPrüfen Sie die Befehlszeile der Aktion...","Fehler...", MessageBoxButtons.OK, MessageBoxIcon.Error);
                return;
                break;
        }     

        //Verbindungen vollständig neu erzeugen (nur ab V2.0.x möglich)
        //new CommandLineInterpreter().Execute("generate /TYPE:CONNECTIONS /REBUILDALLCONNECTIONS:1"); 

        //Verbindungen aktualisieren
        new CommandLineInterpreter().Execute("generate /TYPE:CONNECTIONS"); 

        //Prüflaufschema ausführen
        //new CommandLineInterpreter().Execute("XMsgActionStartVerification");
        new CommandLineInterpreter().Execute("check /TYPE:PROJECT /VERIFICATIONSCHEME:" + "\"" + VERIFYSCHEME + "\"");

        //Anzeige der MV aktualisieren        
        new CommandLineInterpreter().Execute("GfDlgMgrActionReload /DialogName:MsgManagementDlg");

        //Anzeige des Verbindungsnavis altualisieren (wg. evtl. aktivem Filter)
        new CommandLineInterpreter().Execute("GfDlgMgrActionReload /DialogName:XCmPrjDataDialog");
        return;
     }

    [DeclareAction("ESS_CheckAndExport")]
    public void XESSCheckAndExport(string VERIFYSCHEME, string EXPORTACTION)
    {
        //DESCRIPTION:
        //Prüflauf aus Übergabeparameter 'VERIFYSCHEME' ausführen, und danach Drahtkonfektionierungsexport starten (Verlegart abhängig von Parameter 'EXPORTACTION') 
        //Mögliche Werte für Übergabeparameter 'EXPORTACTION' (Stand 2013-06-03 / EPLAN V2.3.2 Build 7293)
        //----------------------------------------------------------------------------------------------------------------------------------------------------------
        //XPWActionExportSelectionToKomax       KOMAX-Export
        //XPWActionExportSelectionToCadCabel    CadCabel-Export
        //XPWActionExportSelectionToSchleuniger Schleuniger-Export
        //XPWActionExportSelectionToPWA         Steinhauer PWA-Export
        //XPWActionExportSelectionToMetzner     Metzner-Export
        //XPWActionExportSelectionToWustec      WuS-TEC-Export
        //XPWActionExportSelectionToWireList    Allgemeine Drahtstückliste-Export
        //XPWShowUnusedExportedWires            Entfallstückliste-Export


        //Anzeige der MV löschen       
        new CommandLineInterpreter().Execute("GfDlgMgrActionIGfWind /DialogName:MsgManagementDlg /function:Clear");
        
        //Prüflaufschema ausführen
        //new CommandLineInterpreter().Execute("XMsgActionStartVerification");
        new CommandLineInterpreter().Execute("check /TYPE:PROJECT /VERIFICATIONSCHEME:" + "\"" + VERIFYSCHEME + "\"");

        //Anzeige der MV aktualisieren        
        new CommandLineInterpreter().Execute("GfDlgMgrActionReload /DialogName:MsgManagementDlg");

        //prüfe Gültigkeit der Action
        CommandLineInterpreter oCLI = new CommandLineInterpreter();
        bool bRet = oCLI.IsExecutable(EXPORTACTION);

        if (bRet)
        {
            //Export-Dialog starten
            new CommandLineInterpreter().Execute(EXPORTACTION);
        }
        else
        {
            MessageBox.Show("Die angegebene Action '" + EXPORTACTION + "'\nim Parameter 'EXPORTACTION' ist unbekannt.", "Fehler", MessageBoxButtons.OK, MessageBoxIcon.Error);
        }

        return;
    }
}