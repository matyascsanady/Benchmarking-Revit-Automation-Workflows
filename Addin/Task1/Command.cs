using Autodesk.Revit.Attributes;
using Autodesk.Revit.DB;
using Autodesk.Revit.UI;

namespace Addin.Task1
{
    [Transaction(TransactionMode.Manual)]
    public class Command : IExternalCommand
    {
        public Result Execute(ExternalCommandData commandData, ref string message, ElementSet elements)
        {
            ViewModel viewModel = new();
            View view = new(viewModel);
            if (!view.ShowDialog() ??  false)
            {
                return Result.Cancelled;
            }

            Transaction t = new(commandData.Application.ActiveUIDocument.Document, "Create sheets");
            t.Start();

            for (int i = 0; i < viewModel.NumberOfSheets; i++)
            {
                ViewSheet sheet = ViewSheet.Create(commandData.Application.ActiveUIDocument.Document, ElementId.InvalidElementId);
                sheet.Name = viewModel.SheetName;
                sheet.LookupParameter("Sheet Number").Set($"{viewModel.SheetNumberPrefix}{viewModel.SheetNumberSeparator}{i.ToString().PadLeft(4, '0')}");
            }

            t.Commit();

            return Result.Succeeded;
        }
    }
}
