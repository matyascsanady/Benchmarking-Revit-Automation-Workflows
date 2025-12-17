using Autodesk.Revit.Attributes;
using Autodesk.Revit.DB;
using Autodesk.Revit.UI;
using System.Collections.ObjectModel;

namespace Addin.Task2
{
    [Transaction(TransactionMode.Manual)]
    public class Command : IExternalCommand
    {
        public Result Execute(ExternalCommandData commandData, ref string message, ElementSet elements)
        {
            Document doc = commandData.Application.ActiveUIDocument.Document;

            ViewModel viewModel = new();
            viewModel.Levels = new ObservableCollection<Level>(
                new FilteredElementCollector(doc)
                    .OfClass(typeof(Level))
                    .Cast<Level>()
                );
            viewModel.Types = new ObservableCollection<TypeModel>(
                new FilteredElementCollector(doc)
                .OfCategory(BuiltInCategory.OST_StructuralColumns)
                .OfClass(typeof(FamilySymbol))
                .Cast<FamilySymbol>()
                .Select(f => new TypeModel(f.LookupParameter("Type Name").AsString(), f.Id))
                );

            View view = new(viewModel);
            if (!view.ShowDialog() ?? false)
            {
                return Result.Cancelled;
            }

            double xSpacingInt = UnitUtils.ConvertToInternalUnits(viewModel.SpacingX, UnitTypeId.Centimeters);
            double ySpacingInt = UnitUtils.ConvertToInternalUnits(viewModel.SpacingY, UnitTypeId.Centimeters);
            double heightOffsetInt = UnitUtils.ConvertToInternalUnits(viewModel.HeightOffset, UnitTypeId.Centimeters);

            using (Transaction t = new(doc, "Create Grids and Columns"))
            {
                t.Start();

                List<ElementId> gridIds = [];

                Line lineX = Line.CreateBound(new(0, 0, 0), new((viewModel.NumberOfGridsY - 1) * ySpacingInt, 0, 0));
                Grid gridX = Grid.Create(doc, lineX);
                gridIds.Add(gridX.Id);
                for (int i = 1; i < viewModel.NumberOfGridsX; i++)
                {
                    gridIds.Add(ElementTransformUtils.CopyElement(doc, gridX.Id, new(0, xSpacingInt * i, 0)).FirstOrDefault());
                }

                Line lineY = Line.CreateBound(new(0, 0, 0), new(0, (viewModel.NumberOfGridsX - 1) * xSpacingInt, 0));
                Grid gridY = Grid.Create(doc, lineY);
                gridIds.Add(gridY.Id);
                for (int i = 1; i < viewModel.NumberOfGridsY; i++)
                {
                    gridIds.Add(ElementTransformUtils.CopyElement(doc, gridY.Id, new(ySpacingInt * i, 0, 0)).FirstOrDefault());
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

                FamilySymbol symbol = (FamilySymbol)doc.GetElement(viewModel.SelectedType.Id);
                if (!symbol.IsActive)
                {
                    symbol.Activate();
                }

                foreach (XYZ intersection in uniqueIntersections)
                {
                    doc.Create.NewFamilyInstance(
                       Line.CreateBound(intersection, new(intersection.X, intersection.Y, intersection.Z + heightOffsetInt)),
                       symbol,
                       viewModel.SelectedLevel,
                       Autodesk.Revit.DB.Structure.StructuralType.Column
                    );
                }

                t.Commit();
            }

            return Result.Succeeded;
        }
    }
}
