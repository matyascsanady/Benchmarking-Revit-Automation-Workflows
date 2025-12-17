using Autodesk.Revit.DB;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Runtime.CompilerServices;

namespace Addin.Task2
{
    public class ViewModel : INotifyPropertyChanged
    {
        private int _numberOfGridsX = 1;
        private int _numberOfGridsY = 1;
        private int _spacingX = 1;
        private int _spacingY = 1;
        private ObservableCollection<Level> _levels = [];
        private ObservableCollection<TypeModel> _types = [];
        private int _heightOffset = 800;

        public int NumberOfGridsX
        {
            get => _numberOfGridsX;
            set
            {
                _numberOfGridsX = value;
                OnPropertyChanged();
            }
        }
        public int NumberOfGridsY
        {
            get => _numberOfGridsY;
            set
            {
                _numberOfGridsY = value;
                OnPropertyChanged();
            }
        }
        public int SpacingX
        {
            get => _spacingX;
            set
            {
                _spacingX = value;
                OnPropertyChanged();
            }
        }
        public int SpacingY
        {
            get => _spacingY;
            set
            {
                _spacingY = value;
                OnPropertyChanged();
            }
        }
        public ObservableCollection<Level> Levels
        {
            get => _levels;
            set
            {
                _levels = value;
                OnPropertyChanged();
            }
        }
        public ObservableCollection<TypeModel> Types
        {
            get => _types;
            set
            {
                _types = value;
                OnPropertyChanged();
            }
        }
        public Level SelectedLevel { get; set; }
        public TypeModel SelectedType { get; set; }
        public int HeightOffset
        {
            get => _heightOffset;
            set
            {
                _heightOffset = value;
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
