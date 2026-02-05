public class ESS_SetDefaultMacroFolders
{
    //###########################################################################################
    // ESS_SetDefaultMacroFolders    
    //###########################################################################################
    // Funktion: Pfad für das Einfügen von Fenster- & Symbolmakros vorbelegen
    // EPLAN Software & Service GmbH & Co. KG / Florian Reiter / EPLAN Consulting
    // 2013-01-17 (c) Florian Reiter
    //###########################################################################################
    // ChangeLog:
    //-------------------------------------------------------------------------------------------
    // 2013-01-17   V1.0    REF EPLAN Software & Service GmbH & Co. KG  Ersterstellung Script
    //########################################################################################### 

    [DeclareAction("SetMacroDefaultFolder")]
    public void XSetMacroDefaultFolder(string FOLDER)
    {        
        string sFolder = PathMap.SubstitutePath(FOLDER);
		//check, if folder exists
        if (!System.IO.Directory.Exists(sFolder))
        {
            System.Windows.Forms.MessageBox.Show("Das angegebene Verzeichnis\n'" + sFolder + "'\nexistiert nicht !", "Fehler...", MessageBoxButtons.OK, MessageBoxIcon.Exclamation);            
            return;
        }

        //im Parameter /FOLDER kann der gewünschte Pfad übergeben werden
        Eplan.EplApi.Base.Settings oSettings = new Eplan.EplApi.Base.Settings();
        
        //set last used window-macro folder to custom-folder
        //Settings nur bis V2.3 gültig
        #region V2.3
        //if(oSettings.ExistSetting("USER.FileSelection.PxfWindowMacro.PermamentSelection.FolderName"))
        //{
        //    oSettings.SetStringSetting("USER.FileSelection.PxfWindowMacro.PermamentSelection.FolderName", sFolder, 0);
        //}

        ////set last used symbol-macro folder to custom-folder
        //if(oSettings.ExistSetting("USER.FileSelection.PxfSymbolMacro.PermamentSelection.FolderName"))
        //{
        //    oSettings.SetStringSetting("USER.FileSelection.PxfSymbolMacro.PermamentSelection.FolderName", sFolder, 0);
        //}

        ////set last used "Makro auswählen" folder to custom-folder
        //if (oSettings.ExistSetting("USER.FileSelection.Makro auswählen.PermamentSelection.FolderName"))
        //{
        //    oSettings.SetStringSetting("USER.FileSelection.Makro auswählen.PermamentSelection.FolderName", sFolder, 0);        
        //}

        ////set last used Symbol-Macro-File (1x \ anhängen, da normalerweise der letzte Pfad+Dateiname gespeichert wird) 
        //if (oSettings.ExistSetting("USER.TrDMProject.LastUsedFile.PxfSymbolMacro"))
        //{
        //    oSettings.SetStringSetting("USER.TrDMProject.LastUsedFile.PxfSymbolMacro", sFolder + "\\", 0);
        //}

        ////set last used Window-Macro-File (1x \ anhängen, da normalerweise der letzte Pfad+Dateiname gespeichert wird)
        //if (oSettings.ExistSetting("USER.TrDMProject.LastUsedFile.PxfWindowMacro"))
        //{
        //    oSettings.SetStringSetting("USER.TrDMProject.LastUsedFile.PxfWindowMacro", sFolder + "\\", 0);
        //}
        #endregion

        //Setting ab V2.4
        #region 2.4
        //set last used symbol-macro / window-macro folder to custom-folder
        if (oSettings.ExistSetting("USER.FileSelection.PxfWindowAndSymbolMacros.PermamentSelection.FolderName"))
        {
            oSettings.SetStringSetting("USER.FileSelection.PxfWindowAndSymbolMacros.PermamentSelection.FolderName", sFolder, 0);
        }

        //set last used Symbol-Macro-File (1x \ anhängen, da normalerweise der letzte Pfad+Dateiname gespeichert wird) 
        if (oSettings.ExistSetting("USER.TrDMProject.LastUsedFile.PxfWindowAndSymbolMacros"))
        {
            oSettings.SetStringSetting("USER.TrDMProject.LastUsedFile.PxfWindowAndSymbolMacros", sFolder + "\\", 0);
        }
        #endregion

       

        //open insert macro dialog 
        new CommandLineInterpreter(true).Execute("XGedStartInteractionAction /Name:XMIaInsertMacro");
       
        return;
    }
}