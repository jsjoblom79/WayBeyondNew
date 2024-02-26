using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace WayBeyond.UX
{
    public class MainWindowViewModel : BindableBase
    {
        public MainWindowViewModel()
        {
            NavigateCommand = new RelayCommand<string>(OnNavigation);
        }

        
        private BindableBase _currentViewModel;

        public BindableBase CurrentViewModel
        {
            get { return _currentViewModel; }
            set { SetProperty(ref _currentViewModel, value); }
        }

        #region Methods
        public RelayCommand<string> NavigateCommand { get; private set; }
        public static event Action Exit = delegate { };

        private async void OnNavigation(string nav)
        {
            switch (nav)
            {
                case "exit":
                    Exit();
                    break;
                default:
                    break;
            }
        }
        #endregion
    }
}
