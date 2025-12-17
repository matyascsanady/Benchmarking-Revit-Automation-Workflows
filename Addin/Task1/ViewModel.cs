using System.ComponentModel;
using System.Runtime.CompilerServices;

namespace Addin.Task1
{
    public class ViewModel : INotifyPropertyChanged
    {
        private string sheetName = "sheet name";
        private string sheetNumberPrefix = "AAA_BB_CC";
        private string sheetNumberSeparator = "-";
        private int numberOfSheets = 1;

        public string SheetName
        {
            get => sheetName;
            set
            {
                sheetName = value;
                OnPropertyChanged();
            }
        }
        public string SheetNumberPrefix
        {
            get => sheetNumberPrefix;
            set
            {
                sheetNumberPrefix = value;
                OnPropertyChanged();
            }
        }
        public string SheetNumberSeparator
        {
            get => sheetNumberSeparator;
            set
            {
                sheetNumberSeparator = value;
                OnPropertyChanged();
            }
        }
        public int NumberOfSheets
        {
            get => numberOfSheets;
            set
            {
                numberOfSheets = value;
                OnPropertyChanged();
            }
        }

        public event PropertyChangedEventHandler? PropertyChanged;

        protected void OnPropertyChanged([CallerMemberName] string? propertyName = null)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }
    }
}
