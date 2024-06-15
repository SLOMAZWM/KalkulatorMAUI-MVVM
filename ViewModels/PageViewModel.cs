﻿using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using KalkulatorMAUI_MVVM.Pages;
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
                Console.WriteLine("Error 404");
            }
        }
    }
}
