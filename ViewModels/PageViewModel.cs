using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using KalkulatorMAUI_MVVM.Views;

namespace KalkulatorMAUI_MVVM.ViewModels
{
    public partial class PageViewModel : ObservableObject
    {
        [ObservableProperty]
        private ContentView _currentView;

        [ObservableProperty]
        private bool _isPortrait = true;

        public CalculatorViewModel CalculatorViewModel { get; set; }

        public PageViewModel()
        {
            CalculatorViewModel = new CalculatorViewModel();
            UpdateView();
        }

        [RelayCommand]
        public void ChangeMode(string mode)
        {
            CalculatorViewModel.ChangeModeCommand.Execute(mode);
            UpdateView();
        }

        public void UpdateOrientation(bool isPortrait)
        {
            IsPortrait = isPortrait;
            UpdateView();
        }

        private void UpdateView()
        {
            if (IsPortrait)
            {
                CurrentView = new StandardView
                {
                    BindingContext = CalculatorViewModel
                };
            }
            else
            {
                switch (CalculatorViewModel.CurrentMode)
                {
                    case CalculatorMode.Scientific:
                        CurrentView = new ScientificView
                        {
                            BindingContext = CalculatorViewModel
                        };
                        break;
                    case CalculatorMode.Programmer:
                        CurrentView = new ProgrammerView
                        {
                            BindingContext = CalculatorViewModel
                        };
                        break;
                }
            }
        }
    }
}
