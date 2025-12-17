using Autodesk.Revit.UI;

namespace Addin
{
    public class Application : IExternalApplication
    {
        public Result OnStartup(UIControlledApplication application)
        {
            BuildUI(application);

            return Result.Succeeded;
        }

        public Result OnShutdown(UIControlledApplication application)
        {
            
            return Result.Succeeded;
        }

        private static void BuildUI(UIControlledApplication application)
        {
            application.CreateRibbonTab("Sample Add-in");
            RibbonPanel panel = application.CreateRibbonPanel("Sample Add-in", "sample");

            Type command1Type = typeof(Task1.Command);
            PushButtonData pushButtonData1 = new(command1Type.FullName, "Task 1", command1Type.Assembly.Location, command1Type.FullName);
            panel.AddItem(pushButtonData1);

            Type command2Type = typeof(Task2.Command);
            PushButtonData pushButtonData2 = new(command2Type.FullName, "Task 2", command2Type.Assembly.Location, command2Type.FullName);
            panel.AddItem(pushButtonData2);
        }
    }
}
