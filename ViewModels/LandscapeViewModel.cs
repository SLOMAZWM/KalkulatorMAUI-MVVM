using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;

namespace KalkulatorMAUI_MVVM.ViewModels
{
    public partial class LandscapeViewModel : ObservableObject
    {
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
    }
}
