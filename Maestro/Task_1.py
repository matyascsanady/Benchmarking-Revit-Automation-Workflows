import clr
from typing import Optional

# Prompt user for input
sheet_count = 1  # @mvar(type='int', display='Number of Sheets')
sheet_name = "New Sheet"  # @mvar(type='string', display='Sheet Name')
sheet_prefix = "A"  # @mvar(type='string', display='Sheet Number Prefix')
sheet_separator = "-"  # @mvar(type='string', display='Separator')
start_number = 1  # @mvar(type='int', display='Starting Number')

def get_default_titleblock(doc) -> Optional['Element']:
    """
    Returns the first available title block family symbol in the document.
    """
    titleblock_symbols = FilteredElementCollector(doc).OfCategory(BuiltInCategory.OST_TitleBlocks).WhereElementIsElementType().ToElements()
    if titleblock_symbols:
        return titleblock_symbols[0]
    else:
        return None

def create_sheets(doc, count: int, name: str, prefix: str, separator: str, start_num: int):
    """
    Creates the specified number of sheets with formatted sheet numbers and the given name.
    """
    titleblock = get_default_titleblock(doc)
    if not titleblock:
        print("No title block family found in the project. Cannot create sheets.")
        return

    created_sheets = []
    t = Transaction(doc, "Create Sheets")
    try:
        t.Start()
        for i in range(count):
            sheet_number = f"{prefix}{separator}{str(start_num + i).zfill(4)}"
            # Create the sheet
            new_sheet = ViewSheet.Create(doc, titleblock.Id)
            if not new_sheet:
                print(f"Failed to create sheet {sheet_number}.")
                continue
            new_sheet.Name = name
            new_sheet.SheetNumber = sheet_number
            created_sheets.append(new_sheet)
        t.Commit()
        print(f"Successfully created {len(created_sheets)} sheets.")
        for sheet in created_sheets:
            print(f"Sheet: {sheet.SheetNumber} - {sheet.Name}")
    except Exception as e:
        print(f"Error during sheet creation: {e}")
        t.RollBack()
        raise

# Main execution
if sheet_count > 0:
    create_sheets(doc, sheet_count, sheet_name, sheet_prefix, sheet_separator, start_number)
    
else:
    print("Sheet count must be greater than zero.")
