using Rod.Calculator.Views;
using Prism.Ioc;
using System.Windows;
using Prism.Modularity;
using Rod.Calculator.Standard;

namespace Rod.Calculator
{
    /// <summary>
    /// Interaction logic for App.xaml
    /// </summary>
    public partial class App
    {
        protected override Window CreateShell()
        {
            return Container.Resolve<MainWindow>();
        }

        protected override void RegisterTypes(IContainerRegistry containerRegistry)
        {

        }

        protected override void ConfigureModuleCatalog(IModuleCatalog moduleCatalog)
        {
            moduleCatalog.AddModule<Standard_Calculator_Module>();
        }
    }
}
