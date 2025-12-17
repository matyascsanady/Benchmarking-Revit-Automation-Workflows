from Autodesk.Revit import DB

NUMBER_OF_SHEETS = 1000
SHEET_NAME = "Name"
SHEET_NUMBER_PREFIX = "AAA_BB_CC"
SHEET_NUMBER_SEPARATOR = "-"

t = Transaction(doc, "Create sheets")
t.Start()

for i in range(NUMBER_OF_SHEETS):
	sheet = ViewSheet.Create(doc, ElementId.InvalidElementId)
	sheet.Name = SHEET_NAME
	sheet.LookupParameter("Sheet Number").Set(SHEET_NUMBER_PREFIX + SHEET_NUMBER_SEPARATOR + f"{i:04}")

t.Commit()
