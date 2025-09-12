using System.Windows;
using FindEdge.Presentation.ViewModels;

namespace FindEdge.Presentation.Views
{
    /// <summary>
    /// Fenêtre de détection de doublons
    /// </summary>
    public partial class DuplicatesWindow : Window
    {
        public DuplicatesWindow()
        {
            InitializeComponent();
            
            // TODO: Injecter les services via DI
            DataContext = new DuplicatesViewModel(new MockDuplicateDetector(), new MockExportService());
        }
    }
}
