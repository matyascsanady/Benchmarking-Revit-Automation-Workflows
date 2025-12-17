import clr

# Prompt user for the string to find and the string to replace it with
find_string = ""  # @mvar(type='string', display='String to Find')
replace_string = ""  # @mvar(type='string', display='String to Replace With')

def swap_sheet_number_strings(doc, find_str: str, replace_str: str) -> int:
    """
    Swaps out a given string for a new string in all sheet numbers in the model.

    :param doc: The active Revit document.
    :param find_str: The string to find in sheet numbers.
    :param replace_str: The string to replace it with.
    :return: The number of sheets updated.
    """
    from Autodesk.Revit.DB import BuiltInCategory, FilteredElementCollector, ViewSheet, Transaction

    updated_count = 0
    try:
        t = Transaction(doc, "Swap Sheet Number Strings")
        t.Start()

        # Collect all sheets in the model
        sheets = FilteredElementCollector(doc).OfCategory(BuiltInCategory.OST_Sheets).WhereElementIsNotElementType().ToElements()

        for sheet in sheets:
            sheet_number = sheet.SheetNumber
            if find_str in sheet_number:
                new_number = sheet_number.replace(find_str, replace_str)
                # Only update if the new number is different
                if new_number != sheet_number:
                    sheet.SheetNumber = new_number
                    updated_count += 1

        t.Commit()
    except Exception as e:
        print(f"An error occurred while swapping sheet numbers: {e}")
        t.RollBack()
        raise

    return updated_count

# Main execution
if find_string:
    try:
        count = swap_sheet_number_strings(doc, find_string, replace_string)
        print(f"Updated {count} sheet(s) where '{find_string}' was replaced with '{replace_string}' in the sheet number.")
    except Exception as e:
        print(f"Failed to swap sheet numbers: {e}")
else:
    print("No 'find' string was specified. Please provide a string to search for in sheet numbers.")
