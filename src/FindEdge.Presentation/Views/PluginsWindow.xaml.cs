using System.Windows;
using FindEdge.Presentation.ViewModels;

namespace FindEdge.Presentation.Views
{
    /// <summary>
    /// Fenêtre de gestion des plugins
    /// </summary>
    public partial class PluginsWindow : Window
    {
        public PluginsWindow()
        {
            InitializeComponent();
            
            // TODO: Injecter le ViewModel via DI
            DataContext = new PluginsViewModel(new MockPluginManager());
        }
    }
}
