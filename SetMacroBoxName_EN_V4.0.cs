// V1:      nairolf         05.12.2010
// V2:      Johann Weiher   07.12.2010
// V2.1:    Johann Weiher   07.12.2010
// V2.2:    nairolf         10.05.2011 
// V3.0:	nairolf			16.05.2011
// V3.1:    EPLAN/MUN :  Start nun über Menüpunkt (GED Seitenstruktur an Makrokasten)      
// V3.2:    EPLAN/MUN :  Nun auch setzen der Variante möglich / aus Seitenname (Muss A,B,... sein) (ab EPLAN 2.2)
// V3.3:    EPLAN/MUN :  Neuer Menüpunkt (Im GED) Seitenstrukter als Seitenmakro (Setzt Seiteneigenschaft Makro:Name[1])
// V4.0:    EPLAN/MUN :  Setzen der Makrobeschreibung aus Seitenbeschreibung. ACHTUNG Dazu wird ein seperates Beschriftungsschema genutzt ([ESS] Macroscripthelper)
//                       (REF): Setzen der Version mit aktuellem Datum und Uhrzeit


using System.Collections;
using System.Collections.Specialized;
using System.Collections.Generic;
using System.IO;
using System.Windows.Forms;
using Eplan.EplApi.ApplicationFramework;
using Eplan.EplApi.Gui;
using Eplan.EplApi.Base;
using Eplan.EplApi.Scripting;


public class SetMacroBoxName
{

    [DeclareAction("SetMacroBoxName")]
    public bool MyMacroBoxName(string EXTENSION, string GETNAMEFROMLEVEL, string WHERE)
    {
        //parameter description:
        //----------------------
        //EXTENSION			...	macroname extionsion (e.g. '.ems')
        //GETNAMEFROMLEVEL	...	get macro name form level code (e.g. 1 = Funktionale Zuordnung, 2 = Anlage, 3 = Aufstellungsort, 4 = Einbauort, 5 = Dokumentenart, 6 = Benutzerdefiniert, 7 = Alle Kennzeichen)
        //WHERE ... if Page: Macroname is written in Propertie Macro:Name[1]    if MacroBox: Macroname is written in macrobox, also the variant from pagename (Must be A,B,...)
        try
        {
            string sPages = string.Empty;
            ActionCallingContext oCTX1 = new ActionCallingContext();
            CommandLineInterpreter oCLI1 = new CommandLineInterpreter();
            oCTX1.AddParameter("TYPE", "PAGES");
            oCLI1.Execute("selectionset", oCTX1);
            oCTX1.GetParameter("PAGES", ref sPages);
            string[] sarrPages = sPages.Split(';');

            if (sarrPages.Length > 1)
            {
                MessageBox.Show("Mehr als eine Seite markiert.\nAktion nicht möglich...", "Hinweis...");
                return false;
            }

            #region get macroname
            string sPageName = sarrPages[0];

            //ensure unique level codes:
            //Funktionale Zuordnung -> $
            //Aufstellungsort -> %
            sPageName = sPageName.Replace("==", "$").Replace("++", "%");
            //get location from pagename
            string sMacroBoxName = string.Empty;


            //add needed / wanted structures to macroname
            #region generate macroname
            string[] sNeededLevels = GETNAMEFROMLEVEL.Split('|');
            foreach (string sLevel in sNeededLevels)
            {
                switch (sLevel)
                {
                    case "1":
                        if (sMacroBoxName.EndsWith(@"\"))
                        {
                            sMacroBoxName += ExtractLevelName(sPageName, "$");
                            sMacroBoxName = sMacroBoxName.Replace(".", @"\");
                        }
                        else
                        {
                            sMacroBoxName += "\\" + ExtractLevelName(sPageName, "$");
                            sMacroBoxName = sMacroBoxName.Replace(".", @"\");
                        }
                        break;
                    case "2":
                        if (sMacroBoxName.EndsWith(@"\"))
                        {
                            sMacroBoxName += ExtractLevelName(sPageName, "=");
                            sMacroBoxName = sMacroBoxName.Replace(".", @"\");
                        }
                        else
                        {
                            sMacroBoxName += "\\" + ExtractLevelName(sPageName, "=");
                            sMacroBoxName = sMacroBoxName.Replace(".", @"\");
                        }
                        break;
                    case "3":
                        if (sMacroBoxName.EndsWith(@"\"))
                        {
                            sMacroBoxName = sMacroBoxName + ExtractLevelName(sPageName, "%");
                            sMacroBoxName = sMacroBoxName.Replace(".", @"\");
                        }
                        else
                        {
                            sMacroBoxName = sMacroBoxName + "\\" + ExtractLevelName(sPageName, "%");
                            sMacroBoxName = sMacroBoxName.Replace(".", @"\");
                        }
                        break;
                    case "4":
                        if (sMacroBoxName.EndsWith(@"\"))
                        {
                            sMacroBoxName = sMacroBoxName + ExtractLevelName(sPageName, "+");
                            sMacroBoxName = sMacroBoxName.Replace(".", @"\");
                        }
                        else
                        {
                            sMacroBoxName = sMacroBoxName + "\\" + ExtractLevelName(sPageName, "+");
                            sMacroBoxName = sMacroBoxName.Replace(".", @"\");
                        }
                        break;
                    case "5":
                        if (sMacroBoxName.EndsWith(@"\"))
                        {
                            sMacroBoxName = sMacroBoxName + ExtractLevelName(sPageName, "&");
                            sMacroBoxName = sMacroBoxName.Replace(".", @"\");
                        }
                        else
                        {
                            sMacroBoxName = sMacroBoxName + "\\" + ExtractLevelName(sPageName, "&");
                            sMacroBoxName = sMacroBoxName.Replace(".", @"\");
                        }
                        break;
                    case "6":
                        if (sMacroBoxName.EndsWith(@"\"))
                        {
                            sMacroBoxName = sMacroBoxName + ExtractLevelName(sPageName, "#");
                        }
                        else
                        {
                            sMacroBoxName = sMacroBoxName + "\\" + ExtractLevelName(sPageName, "#");
                        }
                        break;
                    default:
                        break;
                }
            }
            #endregion

            //Clean-up macroname            
            if (sMacroBoxName.EndsWith(@"\"))
            {
                sMacroBoxName = sMacroBoxName.Remove(sMacroBoxName.Length - 1, 1);
            }
            if (sMacroBoxName.StartsWith(@"\"))
            {
                sMacroBoxName = sMacroBoxName.Substring(1);
            }

            if (sMacroBoxName == string.Empty)
            {
                MessageBox.Show("Es konnte kein Makroname ermittelt werden...", "Fehler", MessageBoxButtons.OK, MessageBoxIcon.Error);
                return false;
            }

            sMacroBoxName = sMacroBoxName + EXTENSION;
            #endregion

            #region generate macrovariant
            int iMacroVariant = getMacroVariant(sPageName.Substring(sPageName.Length - 1, 1));
            #endregion

            //set macrobox: macroname
            string quote = "\"";

            CommandLineInterpreter oCLI2 = new CommandLineInterpreter();
            if (WHERE == "Macrobox")
            {
                oCLI2.Execute("XEsSetPropertyAction /PropertyId:23001 /PropertyIndex:0 /PropertyValue:" + quote + sMacroBoxName + quote);
                oCLI2.Execute("XEsSetPropertyAction /PropertyId:23008 /PropertyIndex:0 /PropertyValue:" + iMacroVariant);
                oCLI2.Execute("XEsSetPropertyAction /PropertyId:23002 /PropertyIndex:0 /PropertyValue:" + quote + System.DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss") + quote);
                oCLI2.Execute("XEsSetPagePropertyAction /PropertyId:40115 /PropertyIndex:0 /PropertyValue:" + quote + sMacroBoxName.Replace(@"\", "") + quote);
                oCLI2.Execute("XEsSetPropertyAction /PropertyId:23004 /PropertyIndex:0 /PropertyValue:" + quote + getMacrodescription(sMacroBoxName.Replace(@"\", "")) + quote);
                oCLI2.Execute("XEsSetPagePropertyAction /PropertyId:40115 /PropertyIndex:0 /PropertyValue:" + quote  + quote );
            }
            if (WHERE == "Page")
            {
                oCLI2.Execute("XEsSetPagePropertyAction /PropertyId:11008 /PropertyIndex:1 /PropertyValue:" + quote + sMacroBoxName + quote);
                oCLI2.Execute("XEsSetPagePropertyAction /PropertyId:11911 /PropertyIndex:0 /PropertyValue:" + quote + System.DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss") + quote);
                oCLI2.Execute("XEsSetPagePropertyAction /PropertyId:40115 /PropertyIndex:0 /PropertyValue:" + quote + sMacroBoxName.Replace(@"\", "") + quote);
                oCLI2.Execute("XEsSetPagePropertyAction /PropertyId:11014 /PropertyIndex:1 /PropertyValue:" + quote + getMacrodescription(sMacroBoxName.Replace(@"\", "")) + quote);
                oCLI2.Execute("XEsSetPagePropertyAction /PropertyId:40115 /PropertyIndex:0 /PropertyValue:" + quote + quote);
            }
            return true;
        }
        catch (System.Exception ex)
        {
            MessageBox.Show(ex.Message, "Fehler", MessageBoxButtons.OK, MessageBoxIcon.Error);
            return false;
        }
    }
    //========================================================================================================================================		
    private string ExtractLevelName(string sPage, string sLevel)
    {
        string sLevelName = string.Empty;

        if (sPage.Contains(sLevel))
        {
            //check existing level codes (remove all text of following level code)
            #region Funktionale Zuordnung
            if (sLevel == "$")
            {
                if (sPage.Contains("="))
                {
                    sLevelName = sPage.Substring(sPage.IndexOf(sLevel), sPage.IndexOf("=") - sPage.IndexOf(sLevel));
                    goto DONE;
                }
                if (sPage.Contains("%"))
                {
                    sLevelName = sPage.Substring(sPage.IndexOf(sLevel), sPage.IndexOf("%") - sPage.IndexOf(sLevel));
                    goto DONE;
                }
                if (sPage.Contains("+"))
                {
                    sLevelName = sPage.Substring(sPage.IndexOf(sLevel), sPage.IndexOf("+") - sPage.IndexOf(sLevel));
                    goto DONE;
                }
                if (sPage.Contains("&"))
                {
                    //check if document type is at beginning
                    if (sPage.StartsWith("&"))
                    {
                        //no extracting needed, when document type at beginning						
                    }
                    else
                    {
                        sLevelName = sPage.Substring(sPage.IndexOf(sLevel), sPage.IndexOf("&") - sPage.IndexOf(sLevel));
                        goto DONE;
                    }
                }
                if (sPage.Contains("#"))
                {
                    //check if user-defined is at beginning
                    if (sPage.StartsWith("#"))
                    {
                        //no extracting needed, when user-defined at beginning
                    }
                    else
                    {
                        sLevelName = sPage.Substring(sPage.IndexOf(sLevel), sPage.IndexOf("#") - sPage.IndexOf(sLevel));
                        goto DONE;
                    }
                }
                //no further structure identifier existing
                sLevelName = sPage.Substring(sPage.IndexOf(sLevel), sPage.IndexOf("/") - sPage.IndexOf(sLevel));
            }
            #endregion
            #region Anlage
            if (sLevel == "=")
            {
                if (sPage.Contains("%"))
                {
                    sLevelName = sPage.Substring(sPage.IndexOf(sLevel), sPage.IndexOf("%") - sPage.IndexOf(sLevel));
                    goto DONE;
                }
                if (sPage.Contains("+"))
                {
                    sLevelName = sPage.Substring(sPage.IndexOf(sLevel), sPage.IndexOf("+") - sPage.IndexOf(sLevel));
                    goto DONE;
                }
                if (sPage.Contains("&"))
                {
                    //check if document type is at beginning
                    if (sPage.StartsWith("&"))
                    {
                        //no extracting needed, when document type at beginning						
                    }
                    else
                    {
                        sLevelName = sPage.Substring(sPage.IndexOf(sLevel), sPage.IndexOf("&") - sPage.IndexOf(sLevel));
                        goto DONE;
                    }
                }
                if (sPage.Contains("#"))
                {
                    //check if user-defined is at beginning
                    if (sPage.StartsWith("#"))
                    {
                        //no extracting needed, when user-defined at beginning
                    }
                    else
                    {
                        sLevelName = sPage.Substring(sPage.IndexOf(sLevel), sPage.IndexOf("#") - sPage.IndexOf(sLevel));
                        goto DONE;
                    }
                }

                //no further structure identifier existing
                sLevelName = sPage.Substring(sPage.IndexOf(sLevel), sPage.IndexOf("/") - sPage.IndexOf(sLevel));
            }
            #endregion

            #region Aufstellungsort
            if (sLevel == "%")
            {
                if (sPage.Contains("+"))
                {
                    sLevelName = sPage.Substring(sPage.IndexOf(sLevel), sPage.IndexOf("+") - sPage.IndexOf(sLevel));
                    goto DONE;
                }
                if (sPage.Contains("&"))
                {
                    //check if document type is at beginning
                    if (sPage.StartsWith("&"))
                    {
                        //no extracting needed, when document type at beginning						
                    }
                    else
                    {
                        sLevelName = sPage.Substring(sPage.IndexOf(sLevel), sPage.IndexOf("&") - sPage.IndexOf(sLevel));
                        goto DONE;
                    }
                }
                if (sPage.Contains("#"))
                {
                    //check if user-defined is at beginning
                    if (sPage.StartsWith("#"))
                    {
                        //no extracting needed, when user-defined at beginning
                    }
                    else
                    {
                        sLevelName = sPage.Substring(sPage.IndexOf(sLevel), sPage.IndexOf("#") - sPage.IndexOf(sLevel));
                        goto DONE;
                    }
                }

                //no further structure identifier existing
                sLevelName = sPage.Substring(sPage.IndexOf(sLevel), sPage.IndexOf("/") - sPage.IndexOf(sLevel));
            }
            #endregion

            #region Einbauort
            if (sLevel == "+")
            {
                if (sPage.Contains("&"))
                {
                    //check if document type is at beginning
                    if (sPage.StartsWith("&"))
                    {
                        //no extracting needed, when document type at beginning						
                    }
                    else
                    {
                        sLevelName = sPage.Substring(sPage.IndexOf(sLevel), sPage.IndexOf("&") - sPage.IndexOf(sLevel));
                        goto DONE;
                    }
                }

                if (sPage.Contains("#"))
                {
                    //check if user-defined is at beginning
                    if (sPage.StartsWith("#"))
                    {
                        //no extracting needed, when user-defined at beginning
                    }
                    else
                    {
                        sLevelName = sPage.Substring(sPage.IndexOf(sLevel), sPage.IndexOf("#") - sPage.IndexOf(sLevel));
                        goto DONE;
                    }
                }

                //no further structure identifier existing
                sLevelName = sPage.Substring(sPage.IndexOf(sLevel), sPage.IndexOf("/") - sPage.IndexOf(sLevel));
            }
            #endregion

            #region Dokumentenart
            if (sLevel == "&")
            {
                //check if document type is at beginning
                if (sPage.StartsWith("&"))
                {
                    //check further existing structures						
                    if (sPage.Contains("$"))
                    {
                        sLevelName = sPage.Substring(sPage.IndexOf(sLevel), sPage.IndexOf("$") - sPage.IndexOf(sLevel));
                        goto DONE;
                    }
                    if (sPage.Contains("="))
                    {
                        sLevelName = sPage.Substring(sPage.IndexOf(sLevel), sPage.IndexOf("=") - sPage.IndexOf(sLevel));
                        goto DONE;
                    }
                    if (sPage.Contains("%"))
                    {
                        sLevelName = sPage.Substring(sPage.IndexOf(sLevel), sPage.IndexOf("%") - sPage.IndexOf(sLevel));
                        goto DONE;
                    }
                    if (sPage.Contains("+"))
                    {
                        sLevelName = sPage.Substring(sPage.IndexOf(sLevel), sPage.IndexOf("+") - sPage.IndexOf(sLevel));
                        goto DONE;
                    }
                    if (sPage.Contains("#"))
                    {
                        sLevelName = sPage.Substring(sPage.IndexOf(sLevel), sPage.IndexOf("#") - sPage.IndexOf(sLevel));
                        goto DONE;
                    }

                    //no further structure identifier existing
                    sLevelName = sPage.Substring(sPage.IndexOf(sLevel), sPage.IndexOf("/") - sPage.IndexOf(sLevel));
                    goto DONE;
                }
                else
                {
                    //document typ NOT at beginning
                    if (sPage.Contains("#"))
                    {
                        if (sPage.StartsWith("#"))
                        {
                            //no extracting needed, when user-defined at beginning
                        }
                        else
                        {
                            sLevelName = sPage.Substring(sPage.IndexOf(sLevel), sPage.IndexOf("#") - sPage.IndexOf(sLevel));
                            goto DONE;
                        }
                    }

                    //no further structure identifier existing
                    sLevelName = sPage.Substring(sPage.IndexOf(sLevel), sPage.IndexOf("/") - sPage.IndexOf(sLevel));
                }

            }
            #endregion

            #region Benutzerdefiniert
            if (sLevel == "#")
            {
                //check if user defined is at beginning
                if (sPage.StartsWith("#"))
                {
                    //check further existing structures						
                    if (sPage.Contains("$"))
                    {
                        sLevelName = sPage.Substring(sPage.IndexOf(sLevel), sPage.IndexOf("$") - sPage.IndexOf(sLevel));
                        goto DONE;
                    }
                    if (sPage.Contains("="))
                    {
                        sLevelName = sPage.Substring(sPage.IndexOf(sLevel), sPage.IndexOf("=") - sPage.IndexOf(sLevel));
                        goto DONE;
                    }
                    if (sPage.Contains("%"))
                    {
                        sLevelName = sPage.Substring(sPage.IndexOf(sLevel), sPage.IndexOf("%") - sPage.IndexOf(sLevel));
                        goto DONE;
                    }
                    if (sPage.Contains("+"))
                    {
                        sLevelName = sPage.Substring(sPage.IndexOf(sLevel), sPage.IndexOf("+") - sPage.IndexOf(sLevel));
                        goto DONE;
                    }
                    if (sPage.Contains("&"))
                    {
                        sLevelName = sPage.Substring(sPage.IndexOf(sLevel), sPage.IndexOf("&") - sPage.IndexOf(sLevel));
                        goto DONE;
                    }

                    //no further structure identifier existing
                    sLevelName = sPage.Substring(sPage.IndexOf(sLevel), sPage.IndexOf("/") - sPage.IndexOf(sLevel));
                    goto DONE;
                }
                else
                {
                    //document typ NOT at beginning
                    if (sPage.Contains("#"))
                    {
                        sLevelName = sPage.Substring(sPage.IndexOf(sLevel), sPage.IndexOf("/") - sPage.IndexOf(sLevel));
                        goto DONE;
                    }
                }
            }
            #endregion
        }
    DONE:
        //remove page no		
        if (sLevelName == string.Empty && sPage.Contains(sLevel))
        {
            sLevelName = sPage.Substring(sPage.IndexOf(sLevel), sPage.IndexOf("/") - sPage.IndexOf(sLevel));
        }

        if (sLevelName != string.Empty)
        {
            sLevelName = sLevelName.Replace(sLevel, string.Empty);
        }


        return sLevelName;
    }

    private int getMacroVariant(string Pagename)
    {
        int iMacroVariant = 0;
        switch (Pagename)
        {
            case "A": iMacroVariant = 0; break;
            case "B": iMacroVariant = 1; break;
            case "C": iMacroVariant = 2; break;
            case "D": iMacroVariant = 3; break;
            case "E": iMacroVariant = 4; break;
            case "F": iMacroVariant = 5; break;
            case "G": iMacroVariant = 6; break;
            case "H": iMacroVariant = 7; break;
            case "I": iMacroVariant = 8; break;
            case "J": iMacroVariant = 9; break;
            case "K": iMacroVariant = 10; break;
            case "L": iMacroVariant = 11; break;
            case "M": iMacroVariant = 12; break;
            case "N": iMacroVariant = 13; break;
            case "O": iMacroVariant = 14; break;
            case "P": iMacroVariant = 15; break;
        }
        return iMacroVariant;
    }

    [DeclareMenu()]
    public void CreateMenu()
    {
        ContextMenuLocation oCtxLoc = new ContextMenuLocation();
        //Kontextmenüerweiterung im Seiten-Navi:

        oCtxLoc.DialogName = "Editor";
        oCtxLoc.ContextMenuName = "Ged";
        Eplan.EplApi.Gui.ContextMenu oCTXMnu = new Eplan.EplApi.Gui.ContextMenu();
        oCTXMnu.AddMenuItem(oCtxLoc, "Page structure to Macro box", "SetMacroBoxName /EXTENSION:\".ema\" /GETNAMEFROMLEVEL:1|2|3|4|6 /WHERE:Macrobox", true, false);
        oCTXMnu.AddMenuItem(oCtxLoc, "Page structure as page macro", "SetMacroBoxName /EXTENSION:\".emp\" /GETNAMEFROMLEVEL:1|2|3|4|6 /WHERE:Page", false, false);
        //oCtxLoc.DialogName = "PmPageObjectTreeDialog";
        //oCtxLoc.ContextMenuName = "1007";
        //oCTXMnu.AddMenuItem(oCtxLoc, "Seitenstruktur als Seitenmakro", "SetMacroBoxName /EXTENSION:\".emp\" /GETNAMEFROMLEVEL:1|2|3|4|6 /WHERE:Page", false, false);
    }

    public string getMacrodescription(string filter)
    {
        Settings oSett = new Settings();
        oSett.SetStringSetting("USER.Labelling.Config.[ESS] Macroscripthelper.Data.SortFilter.FilterSchemeData", "0|1|0|40115;0|0|" + filter + "|0|1|1|0|0|0;0|", 0);
        ActionCallingContext labellingContext1 = new ActionCallingContext();
        labellingContext1.AddParameter("CONFIGSCHEME", "[ESS] Macroscripthelper");
        labellingContext1.AddParameter("DESTINATIONFILE", @"$(DOC)\Macroscripthelper.txt");
        labellingContext1.AddParameter("LANGUAGE", "de_DE");
        labellingContext1.AddParameter("USESELECTION ", "1");
        labellingContext1.AddParameter("SHOWOUTPUT", "0");
        labellingContext1.AddParameter("RECREPEAT", "1");
        labellingContext1.AddParameter("SORTSCHEME", "");
        labellingContext1.AddParameter("TASKREPEAT", "1");
        new CommandLineInterpreter().Execute("label", labellingContext1);


        //Seitenbeschreibung der aktuellen Seite auslesen
        string Macroscripthelper = PathMap.SubstitutePath(@"$(DOC)") + @"\Macroscripthelper.txt";
        //System.Collections.Specialized.StringCollection scLocations = new System.Collections.Specialized.StringCollection();
        string Desciption = "";
        System.IO.StreamReader oSr = new System.IO.StreamReader(Macroscripthelper, true);
        if (System.IO.File.Exists(Macroscripthelper))
        {

            while (!oSr.EndOfStream)
            {
               Desciption = oSr.ReadLine();
            }

            oSr.Close();
            System.IO.File.Delete(Macroscripthelper);

        }

        return Desciption;
    }
}