import Autodesk.Revit.DB

X_NUMBER_OF_GRIDS = 10
Y_NUMBER_OF_GRIDS = 5
X_SPACING_CM = 500
Y_SPACING_CM = 1000
TYPE_NAME = "Column_CIP_60x60"
LEVEL_NAME = "L0 - Ground Floor"
HEIGHT_OFFSET_CM = 800

x_spacing_int = UnitUtils.ConvertToInternalUnits(X_SPACING_CM, UnitTypeId.Centimeters)
y_spacing_int = UnitUtils.ConvertToInternalUnits(Y_SPACING_CM, UnitTypeId.Centimeters)
height_offset_int = UnitUtils.ConvertToInternalUnits(HEIGHT_OFFSET_CM, UnitTypeId.Centimeters)

t = Transaction(doc, "Create Grids and Columns")
t.Start()

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


symbols = FilteredElementCollector(doc).OfCategory(BuiltInCategory.OST_StructuralColumns).OfClass(FamilySymbol)
symbol = None
for s in symbols:
	if s.LookupParameter("Type Name").AsString() == TYPE_NAME:
		symbol = s
		break

if not symbol.IsActive:
	symbol.Activate()
	
levels = FilteredElementCollector(doc).OfClass(Level)
level = None
for l in levels:
	if l.Name == LEVEL_NAME:
		level = l
		break


for point in unique_intersections:
	doc.Create.NewFamilyInstance(Line.CreateBound(point, XYZ(point.X, point.Y, height_offset_int)), symbol, level, Structure.StructuralType.Column)

t.Commit()
