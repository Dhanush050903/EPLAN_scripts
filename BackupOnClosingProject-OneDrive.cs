
/* ================================================================================================
 * ETO Digital & SMART(EDS)
 * Power Systems Division
 * Energy Management Business
 * Bharat Chaklasiya
 * ================================================================================================
*/

using System;
using System.IO;
using System.Windows.Forms;



public class Backup
{
	
	[DeclareEventHandler("onActionStart.String.XPrjActionProjectClose")] //Action is defined at Project close button command

  
  public void Function()
  {
    string strFullProjectname = PathMap.SubstitutePath("$(P)"); //Default path 


    string projectsPath = @"C:\EPLAN\Backups";
    string projectName = PathMap.SubstitutePath("$(PROJECTNAME)"); //Default project name

    string date = DateTime.Now.ToString("yyyy-MM-dd"); //Defined date variable

    string backupDirectory = Path.Combine(projectsPath); //Defined backupDirectory
    string backupFileName = projectName + "_" + date + ".zw1"; //Backup file name = ProjectName_date_time.zw1
    string backupFullFileName = Path.Combine(backupDirectory, backupFileName); //Backup file at  "/Backup" folder
	
	
	//For user to see dialog message box below
	DialogResult Result = MessageBox.Show(
			"Backup project on exit?\n'"
			+ backupFullFileName +
			"'be generated?",
			"Data backup",
			MessageBoxButtons.YesNo,
			MessageBoxIcon.Question
			);

	if (Result == DialogResult.Yes)
	
	{
		
		//Process view
		if (!Directory.Exists(backupDirectory))
		{
		Directory.CreateDirectory(backupDirectory);
		}

		Progress progress = new Progress("SimpleProgress");
		progress.SetAllowCancel(true);
		progress.SetAskOnCancel(true);
		progress.BeginPart(100, "");
		progress.SetTitle("Backup");
		progress.ShowImmediately();

		if (!progress.Canceled())
		{
		CommandLineInterpreter cli = new CommandLineInterpreter();
		ActionCallingContext acc = new ActionCallingContext();

		acc.AddParameter("BACKUPAMOUNT", "BACKUPAMOUNT_ALL"); // The entire contents of the project directory are backed up.
		acc.AddParameter("BACKUPMEDIA", "DISK"); // DISK: Project is saved on a hard drive, disk, etc
		acc.AddParameter("BACKUPMETHOD", "BACKUP"); // BACKUP: Project is backed up
		acc.AddParameter("COMPRESSPRJ", "1"); // should be compressed before the backup (optional, 0 = No, 1 = Yes).
		acc.AddParameter("INCLEXTDOCS", "1"); //external documents should be included in the backup (optional, 0 = No, 1 = Yes).
		acc.AddParameter("INCLIMAGES", "1"); //image files should be included in the backup (optional, 0 = No, 1 = Yes).
		acc.AddParameter("ARCHIVENAME", backupFileName); //Backup file name defined
		acc.AddParameter("DESTINATIONPATH", backupDirectory);//Backup file directory defined
		acc.AddParameter("TYPE", "PROJECT"); // Type Project

		cli.Execute("backup", acc); //all above parameter defined action execute cammand
		}

		//For user to see dialog message box below
		MessageBox.Show(
        "Backup was created successfully:" +
        Environment.NewLine +
        backupFullFileName,
        "Note",
        MessageBoxButtons.OK,
        MessageBoxIcon.Information
        );

    progress.EndPart(true);
	
	return;

  }
  }
}
