using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using KalkulatorMAUI_MVVM.Pages;
using KalkulatorMAUI_MVVM.Views;

namespace KalkulatorMAUI_MVVM.ViewModels
{
    public enum CalculatorMode
    {
        Scientific,
        Programmer,
        Currency,
        Graph
    }

    public partial class PageViewModel : ObservableObject
    {
        public CalculatorViewModel CalculatorViewModel { get; set; }

        [ObservableProperty]
        private ContentView _currentView;

        [ObservableProperty]
        private bool _isPortrait = true;

        [ObservableProperty]
        private bool _isMenuVisible = false;

        [ObservableProperty]
        private bool _isModeCalcVisible = true;

        [ObservableProperty]
        private CalculatorMode _currentMode = CalculatorMode.Scientific;

        [RelayCommand]
        private void ChangeMode(string mode)
        {
            switch (mode)
            {
                case "Scientific":
                    CurrentMode = CalculatorMode.Scientific;
                    break;
                case "Programmer":
                    CurrentMode = CalculatorMode.Programmer;
                    break;
            }

            UpdateMode();
        }

        public PageViewModel()
        {
            CalculatorViewModel = new CalculatorViewModel();
            CurrentView = new ScientificView();
            UpdateView();
        }

        public void UpdateOrientation(bool isPortrait)
        {
            IsPortrait = isPortrait;
            UpdateView();
            NavigateToAppropriatePage();
        }

        private async void NavigateToAppropriatePage()
        {
            if (IsPortrait)
            {
                await Shell.Current.GoToAsync("//MainPage");
            }
            else
            {
                await Shell.Current.GoToAsync("//LandscapePage");
            }
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
                UpdateMode();
            }
        }

        private void UpdateMode()
        {
            if (CalculatorMode.Scientific.Equals(CurrentMode))
            {
                CurrentView = new ScientificView
                {
                    BindingContext = CalculatorViewModel
                };
            }
            else if (CalculatorMode.Programmer.Equals(CurrentMode))
            {
                CurrentView = new ProgrammerView
                {
                    BindingContext = new ProgrammerViewModel()
                };
            }
        }

        [RelayCommand]
        private void MenuClicked()
        {
            IsMenuVisible = !IsMenuVisible;
            IsModeCalcVisible = !IsModeCalcVisible;
        }
    }
}
