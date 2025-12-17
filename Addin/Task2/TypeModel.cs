using Autodesk.Revit.DB;

namespace Addin.Task2
{
    public class TypeModel(string name, ElementId id)
    {
        public string Name => name;
        public ElementId Id => id;
    }
}
