using Prism.Mvvm;

namespace Rod.Calculator.ViewModels
{
    public class MainWindowViewModel : BindableBase
    {
        private string _title = "Calculator";
        public string Title
        {
            get { return _title; }
            set { SetProperty(ref _title, value); }
        }

        public MainWindowViewModel()
        {

        }
    }
}
