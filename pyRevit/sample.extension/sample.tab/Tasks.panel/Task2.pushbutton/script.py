import clr
from Autodesk.Revit.DB import *
from pyrevit import forms, revit

doc = revit.HOST_APP.doc

class ColumnTypeOption(forms.TemplateListItem):
    @property
    def name(self):
        return f"{self.item.LookupParameter('Type Name').AsString()}"

X_NUMBER_OF_GRIDS = int(forms.ask_for_number_slider(
	min=1,
	max=500,
	prompt="Numbe of Grids in X"
))
Y_NUMBER_OF_GRIDS = int(forms.ask_for_number_slider(
	min=1,
	max=500,
	prompt="Numbe of Grids in Y"
))
X_SPACING_CM = int(forms.ask_for_number_slider(
	min=1,
	max=3000,
	prompt="Spacing in X [cm]"
))
Y_SPACING_CM = int(forms.ask_for_number_slider(
	min=1,
	max=3000,
	prompt="Spacing in Y [cm]"
))
LEVEL = forms.SelectFromList.show(
    FilteredElementCollector(doc).OfClass(Level),
 	name_attr="Name"
)
COLUMN_TYPE = forms.SelectFromList.show(
    [ColumnTypeOption(sym) for sym in FilteredElementCollector(doc).OfCategory(BuiltInCategory.OST_StructuralColumns).OfClass(FamilySymbol)]
)
HEIGHT_OFFSET_CM = int(forms.ask_for_number_slider(
	min=1,
	max=2000,
	prompt="Height offset [cm]"
))

x_spacing_int = UnitUtils.ConvertToInternalUnits(X_SPACING_CM, UnitTypeId.Centimeters)
y_spacing_int = UnitUtils.ConvertToInternalUnits(Y_SPACING_CM, UnitTypeId.Centimeters)
height_offset_int = UnitUtils.ConvertToInternalUnits(HEIGHT_OFFSET_CM, UnitTypeId.Centimeters)

with revit.Transaction(name="Create Grids and Columns", doc=doc):
    
	grid_ids = []

	line_x = Line.CreateBound(XYZ(0,0,0), XYZ((Y_NUMBER_OF_GRIDS-1)*y_spacing_int,0,0))
	grid_x = Grid.Create(doc, line_x)
	grid_x.Name = "A"
	grid_ids.Add(grid_x.Id)
	for i in range(1, X_NUMBER_OF_GRIDS):
		copy = ElementTransformUtils.CopyElement(doc, grid_x.Id, XYZ(0,x_spacing_int*i,0))[0]
		grid_ids.Add(copy)

	line_y = Line.CreateBound(XYZ(0,0,0), XYZ(0,(X_NUMBER_OF_GRIDS-1)*x_spacing_int,0))
	grid_y = Grid.Create(doc, line_y)
	grid_y.Name = "1"
	grid_ids.Add(grid_y.Id)
	for i in range(1, Y_NUMBER_OF_GRIDS):
		copy = ElementTransformUtils.CopyElement(doc, grid_y.Id, XYZ(y_spacing_int*i,0,0))[0]
		grid_ids.Add(copy)


	grid_lines = [doc.GetElement(x).Curve for x in grid_ids]
	intersections = []
	for g1 in grid_lines:
		for g2 in grid_lines:
			if g1 == g2:
				continue
		
			ira = clr.Reference[IntersectionResultArray]()
			comp = g1.Intersect(g2, ira)
			if comp == SetComparisonResult.Overlap:
				arr = ira.Value
				for i in range(arr.Size):
					intersections.append(arr.get_Item(i).XYZPoint)


	unique_intersections = list({(round(p.X,6), round(p.Y,6), round(p.Z,6)): p for p in intersections}.values())

	if not COLUMN_TYPE.IsActive:
		COLUMN_TYPE.Activate()

	for point in unique_intersections:
		doc.Create.NewFamilyInstance(Line.CreateBound(point, XYZ(point.X, point.Y, height_offset_int)), COLUMN_TYPE, LEVEL, Structure.StructuralType.Column)
