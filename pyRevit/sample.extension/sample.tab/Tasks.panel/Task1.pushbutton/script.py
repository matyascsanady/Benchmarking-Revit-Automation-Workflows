from Autodesk.Revit.DB import *
from pyrevit import forms, revit

doc = revit.HOST_APP.doc

NUMBER_OF_SHEETS = int(forms.ask_for_number_slider(
	min=1,
	max=2000,
	prompt="Amount of sheets"
))
SHEET_NAME = forms.ask_for_string(
	default="sheet name",
	prompt="Sheet names"
)
SHEET_NUMBER_PREFIX = forms.ask_for_string(
	default="AAA_BB_CC",
	prompt="Sheet number prefix"
)
SHEET_NUMBER_SEPARATOR = forms.ask_for_string(
	default="-",
	prompt="Sheet number separator"
)

with revit.Transaction(name="Create Sheets", doc=doc):

	for i in range(NUMBER_OF_SHEETS):
		sheet = ViewSheet.Create(doc, ElementId.InvalidElementId)
		sheet.Name = SHEET_NAME
		sheet.LookupParameter("Sheet Number").Set(SHEET_NUMBER_PREFIX + SHEET_NUMBER_SEPARATOR + f"{i:04}")
