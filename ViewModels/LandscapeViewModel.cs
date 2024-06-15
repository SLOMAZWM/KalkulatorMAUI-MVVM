using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using KalkulatorMAUI_MVVM.Views;

namespace KalkulatorMAUI_MVVM.ViewModels
{
    public partial class LandscapeViewModel : ObservableObject
    {
        [ObservableProperty]
        private ContentView _currentView;

        public CalculatorViewModel CalculatorViewModel { get; set; }

        public LandscapeViewModel()
        {
            CalculatorViewModel = new CalculatorViewModel();
        }

        [RelayCommand]
        public void BackToPortrait()
        {
            Shell.Current.GoToAsync("//MainPage");
        }

        private void UpdateView()
        {
            CurrentView = new ScientificView
            {
                BindingContext = CalculatorViewModel
            };
        }
    }
}
