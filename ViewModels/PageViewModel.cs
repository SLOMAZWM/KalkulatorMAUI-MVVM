﻿using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using KalkulatorMAUI_MVVM.Enums;
using KalkulatorMAUI_MVVM.Models;
using KalkulatorMAUI_MVVM.Pages;
using KalkulatorMAUI_MVVM.Views;
using System.Collections.ObjectModel;

namespace KalkulatorMAUI_MVVM.ViewModels
{
    public partial class PageViewModel : ObservableObject
    {
        [ObservableProperty]
        private ContentView _currentView;

        [ObservableProperty]
        private ObservableCollection<HistoryOperation> _historyOperations = new ObservableCollection<HistoryOperation>();

        [ObservableProperty]
        private bool _isDisplayVisible = true;

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
                    IsDisplayVisible = true;
                    break;
                case "Programmer":
                    CurrentMode = CalculatorMode.Programmer;
                    IsDisplayVisible = false;
                    break;
                case "Currency":
                    CurrentMode = CalculatorMode.Currency;
                    IsDisplayVisible = false;
                    break;
            }

            UpdateMode();
        }

        public void UpdateOrientation(bool isPortrait)
        {
            IsPortrait = isPortrait;
            UpdateView();
            NavigateToAppropriatePage();
        }

        private void NavigateToAppropriatePage()
        {
            if (IsPortrait)
            {
                Shell.Current.GoToAsync("//MainPage");
            }
            else
            {
                Shell.Current.GoToAsync("//LandscapePage");
            }
        }

        private void UpdateView()
        {
            if (IsPortrait)
            {
                CurrentView = new StandardView
                {
                    BindingContext = new CalculatorViewModel(this)
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
                    BindingContext = new CalculatorViewModel(this)
                };
            }
            else if (CalculatorMode.Programmer.Equals(CurrentMode))
            {
                CurrentView = new ProgrammerView
                {
                    BindingContext = new ProgrammerViewModel(this)
                };
            }
            else if(CalculatorMode.Currency.Equals(CurrentMode))
            {
                CurrentView = new CurrencyView
                {
                    BindingContext = new CurrencyViewModel(this)
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
