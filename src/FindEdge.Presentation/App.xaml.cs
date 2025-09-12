using System.Windows;
using FindEdge.Presentation.ViewModels;
using FindEdge.Presentation.Views;

namespace FindEdge.Presentation
{
    /// <summary>
    /// Application WPF FindEdge
    /// </summary>
    public partial class App : Application
    {
        protected override void OnStartup(StartupEventArgs e)
        {
            base.OnStartup(e);
            
            // Configuration de l'application
            Current.ShutdownMode = ShutdownMode.OnExplicitShutdown;
        }
    }
}