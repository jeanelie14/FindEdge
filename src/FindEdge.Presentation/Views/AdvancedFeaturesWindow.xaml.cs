using System;
using System.Windows;
using FindEdge.Presentation.ViewModels;

namespace FindEdge.Presentation.Views
{
    /// <summary>
    /// Fenêtre des fonctionnalités avancées de FindEdge Professional
    /// </summary>
    public partial class AdvancedFeaturesWindow : Window
    {
        private readonly AdvancedFeaturesViewModel _viewModel;

        public AdvancedFeaturesWindow(AdvancedFeaturesViewModel viewModel)
        {
            InitializeComponent();
            _viewModel = viewModel ?? throw new ArgumentNullException(nameof(viewModel));
            DataContext = _viewModel;
        }

        private void Window_Loaded(object sender, RoutedEventArgs e)
        {
            // Initialisation de la fenêtre
            SearchQueryTextBox.Focus();
        }

        private void Window_Closing(object sender, System.ComponentModel.CancelEventArgs e)
        {
            // Nettoyage avant fermeture
            _viewModel?.Dispose();
        }
    }
}
