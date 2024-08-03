using Prism.Mvvm;

namespace Rod.Calculator.ViewModels
{
    public class MainWindowViewModel : BindableBase
    {
        ////////////////////////////////////////////////////////////////////////////////////////////////////
        #region Title

        private string _title = "Calculator";
        public string Title
        {
            get => _title;
            set => SetProperty(ref _title, value);
        }

        #endregion Title
        ////////////////////////////////////////////////////////////////////////////////////////////////////

        public MainWindowViewModel()
        {
        }
    }
}
