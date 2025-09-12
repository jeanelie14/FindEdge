using System.Windows;
using FindEdge.Presentation.ViewModels;

namespace FindEdge.Presentation.Views
{
    /// <summary>
    /// Fenêtre principale de l'application FindEdge
    /// </summary>
    public partial class MainWindow : Window
    {
        public MainWindow()
        {
            InitializeComponent();
            
            // Initialiser le ViewModel
            DataContext = new MainViewModel(new MockHybridSearchEngine());
        }
    }
}
