using System;
using System.Windows;
using FindEdge.Presentation.ViewModels;

namespace FindEdge.Presentation.Views
{
    /// <summary>
    /// Fenêtre de gestion de l'index de recherche
    /// </summary>
    public partial class IndexWindow : Window
    {
        public IndexWindow()
        {
            InitializeComponent();
            
            // TODO: Injecter le ViewModel via DI
            DataContext = new IndexViewModel(new MockIndexManager());
        }

        private void AddDirectory_Click(object sender, RoutedEventArgs e)
        {
            if (!string.IsNullOrWhiteSpace(NewDirectoryTextBox.Text))
            {
                var viewModel = (IndexViewModel)DataContext;
                viewModel.IndexConfiguration.IndexedDirectories.Add(NewDirectoryTextBox.Text);
                viewModel.IndexedDirectories.Add(NewDirectoryTextBox.Text);
                NewDirectoryTextBox.Clear();
            }
        }
    }
}
