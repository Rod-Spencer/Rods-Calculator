using Prism.Ioc;
using Prism.Modularity;
using Prism.Regions;
using Rod.Calculator.Standard.Views;

namespace Rod.Calculator.Standard
{
    public class Standard_Calculator_Module : IModule
    {
        private readonly IRegionManager _regionManager;

        public Standard_Calculator_Module(IRegionManager regionManager)
        {
            _regionManager = regionManager;
        }

        public void OnInitialized(IContainerProvider containerProvider)
        {
            IRegion region = _regionManager.Regions["ContentRegion"];

            var view = containerProvider.Resolve<Standard_View>();
            region.Add(view);
        }

        public void RegisterTypes(IContainerRegistry containerRegistry)
        {
        }
    }
}
