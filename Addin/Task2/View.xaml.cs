using System.Windows;

namespace Addin.Task2
{
    public partial class View : Window
    {
        public View(ViewModel viewModel)
        {
            DataContext = viewModel;
            InitializeComponent();
        }

        private void Button_Click(object sender, RoutedEventArgs e)
        {
            DialogResult = true;
        }
    }
}
