using Eplan.EplApi.ApplicationFramework;
public class Script
{
    [Start]
    public bool OpenProject()
    {   
        Progress progress = new Progress("Open Project");
        progress.SetAllowCancel(true);
        
        progress.BeginPart(100, "Opening Project...");

        ActionCallingContext context = new ActionCallingContext();
        context.AddParameter("Project", @"C:\EPLAN\Projects\6200083 - Daventry.elk");
        
        bool result = false;

        try{
            if (!progress.Canceled())
            {
                result= new CommandLineInterpreter().Execute("ProjectOpen", context);
            }
        }
        finally{
            progress.EndPart(true);
        }
        return result;

    }
}

