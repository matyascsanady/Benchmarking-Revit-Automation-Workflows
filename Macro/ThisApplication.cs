using Autodesk.Revit.DB;
using Autodesk.Revit.UI;

namespace Tasks
{
   [Autodesk.Revit.Attributes.Transaction(Autodesk.Revit.Attributes.TransactionMode.Manual)]
   [Autodesk.Revit.DB.Macros.AddInId("492CFAA8-2F25-40F9-A272-552A8648C475")]
   public partial class ThisApplication
   {
      private void Module_Startup(object? sender, EventArgs e)
      {
      }

      private void Module_Shutdown(object? sender, EventArgs e)
      {
      }
      
      public void Task1()
      {
         int NUMBER_OF_SHEETS = 10000;
         char SHEET_NUMBER_SPEARATOR = '-';
         string SHEET_NUMBER_PREFIX = "AAA_BB_CC";
         string SHEET_NAME = "sheet name";

         using (Transaction t = new(ActiveUIDocument.Document, "Create Sheets"))
         {
            t.Start();

            for (int i = 0; i < NUMBER_OF_SHEETS; i++)
            {
               ViewSheet sheet = ViewSheet.Create(ActiveUIDocument.Document, ElementId.InvalidElementId);
               sheet.Name = SHEET_NAME;
               sheet.LookupParameter("Sheet Number").Set($"{SHEET_NUMBER_PREFIX}{SHEET_NUMBER_SPEARATOR}{i.ToString().PadLeft(4, '0')}");
            }

            t.Commit();
         }
      }

      public void Task2()
      {
         Document doc = ActiveUIDocument.Document;
         
         int X_NUMBER_OF_GRIDS = 15;
         int Y_NUMBER_OF_GRIDS = 5;
         int X_SPACING_CM = 250;
         int Y_SPACING_CM = 1000;
         string TYPE_NAME = "Column_CIP_60x60";
         string LEVEL_NAME = "L0 - Ground Floor";
         int HEIGHT_OFFSET_CM = 800;

         double xSpacingInt = UnitUtils.ConvertToInternalUnits(X_SPACING_CM, UnitTypeId.Centimeters);
         double ySpacingInt = UnitUtils.ConvertToInternalUnits(Y_SPACING_CM, UnitTypeId.Centimeters);
         double heightOffsetInt = UnitUtils.ConvertToInternalUnits(HEIGHT_OFFSET_CM, UnitTypeId.Centimeters);

         using (Transaction t = new(doc, "Create Grids and Columns"))
         {
            t.Start();

            List<ElementId> gridIds = [];

            Line lineX = Line.CreateBound(new(0,0,0), new((Y_NUMBER_OF_GRIDS-1)*ySpacingInt,0,0));
            Grid gridX = Grid.Create(doc, lineX);
            gridIds.Add(gridX.Id);
            for (int i=1; i < X_NUMBER_OF_GRIDS; i++)
            {
               gridIds.Add(ElementTransformUtils.CopyElement(doc, gridX.Id, new(0, xSpacingInt*i, 0)).FirstOrDefault());
            }

            Line lineY = Line.CreateBound(new(0,0,0), new(0, (X_NUMBER_OF_GRIDS-1)*xSpacingInt,0));
            Grid gridY = Grid.Create(doc, lineY);
            gridIds.Add(gridY.Id);
            for (int i=1; i < Y_NUMBER_OF_GRIDS; i++)
            {
               gridIds.Add(ElementTransformUtils.CopyElement(doc, gridY.Id, new(ySpacingInt*i, 0, 0)).FirstOrDefault());
            }

            List<XYZ> intersections = [];
            foreach (Curve curve1 in gridIds.Select(x => ((Grid)doc.GetElement(x)).Curve))
            {
               foreach (Curve curve2 in gridIds.Select(x => ((Grid)doc.GetElement(x)).Curve))
               {
                  if (curve1 == curve2) { continue; }

                  if (curve1.Intersect(curve2, out IntersectionResultArray resultArray) == SetComparisonResult.Overlap)
                  {
                        for (int i = 0; i < resultArray.Size; i++)
                        {
                           intersections.Add(resultArray.get_Item(i).XYZPoint);
                        }
                  }
               }
            }

            List<XYZ> uniqueIntersections = intersections
               .GroupBy(p => (
                  X: Math.Round(p.X, 6),
                  Y: Math.Round(p.Y, 6),
                  Z: Math.Round(p.Z, 6)))
               .Select(g => g.First())
               .ToList();

            FamilySymbol symbol = new FilteredElementCollector(doc)
               .OfCategory(BuiltInCategory.OST_StructuralColumns)
               .OfClass(typeof(FamilySymbol))
               .Where(x => x.LookupParameter("Type Name").AsString() == TYPE_NAME)
               .Cast<FamilySymbol>()
               .First();

            if (!symbol.IsActive)
            {
               symbol.Activate();
            }

            Level level = new FilteredElementCollector(doc)
               .OfClass(typeof(Level))
               .Where(x => x.Name == LEVEL_NAME)
               .Cast<Level>()
               .First();

            foreach (XYZ intersection in uniqueIntersections)
            {
               doc.Create.NewFamilyInstance(
                  Line.CreateBound(intersection, new(intersection.X, intersection.Y, intersection.Z+heightOffsetInt)),
                  symbol,
                  level,
                  Autodesk.Revit.DB.Structure.StructuralType.Column
               );
            }

            t.Commit();
         }
      }
   }
}
