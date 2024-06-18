using KalkulatorMAUI_MVVM.Models;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using System.Collections.Generic;
using System.Linq;

namespace KalkulatorMAUI_MVVM.ViewModels
{
    public enum NumberSystem
    {
        HEX,
        DEC,
        OCT,
        BIN,
    }

    public partial class ProgrammerViewModel : ObservableObject
    {
        [ObservableProperty]
        private NumberSystem _selectedNumberSystem;

        [ObservableProperty]
        private string _display = "0";

        private bool _firstSign = true;

        [ObservableProperty]
        private ButtonState _buttonState;

        [ObservableProperty]
        private List<NumberSystem> _numberSystems;

        public ProgrammerViewModel()
        {
            NumberSystems = Enum.GetValues(typeof(NumberSystem)).Cast<NumberSystem>().ToList();
            ButtonState = new ButtonState();
            SelectedNumberSystem = NumberSystem.DEC;
        }

        [RelayCommand]
        private void EnterToDisplay(string Sign)
        {
            if (_firstSign)
            {
                Display = Sign;
                _firstSign = false;
            }
            else
            {
                Display += Sign;
            }
        }

        [RelayCommand]
        private void ClearDisplay()
        {
            Display = "0";
            _firstSign = true;
        }

        partial void OnSelectedNumberSystemChanged(NumberSystem value)
        {
            switch (SelectedNumberSystem)
            {
                case NumberSystem.HEX:
                    EnableHexButtons();
                    break;
                case NumberSystem.DEC:
                    EnableDecButtons();
                    break;
                case NumberSystem.OCT:
                    EnableOctButtons();
                    break;
                case NumberSystem.BIN:
                    EnableBinButtons();
                    break;
            }
        }

        private void EnableHexButtons()
        {
            ButtonState.IsAEnabled = true;
            ButtonState.IsBEnabled = true;
            ButtonState.IsCEnabled = true;
            ButtonState.IsDEnabled = true;
            ButtonState.IsEEnabled = true;
            ButtonState.IsFEnabled = true;
            ButtonState.Is0Enabled = true;
            ButtonState.Is1Enabled = true;
            ButtonState.Is2Enabled = true;
            ButtonState.Is3Enabled = true;
            ButtonState.Is4Enabled = true;
            ButtonState.Is5Enabled = true;
            ButtonState.Is6Enabled = true;
            ButtonState.Is7Enabled = true;
            ButtonState.Is8Enabled = true;
            ButtonState.Is9Enabled = true;
        }

        private void EnableDecButtons()
        {
            ButtonState.IsAEnabled = false;
            ButtonState.IsBEnabled = false;
            ButtonState.IsCEnabled = false;
            ButtonState.IsDEnabled = false;
            ButtonState.IsEEnabled = false;
            ButtonState.IsFEnabled = false;
            ButtonState.Is0Enabled = true;
            ButtonState.Is1Enabled = true;
            ButtonState.Is2Enabled = true;
            ButtonState.Is3Enabled = true;
            ButtonState.Is4Enabled = true;
            ButtonState.Is5Enabled = true;
            ButtonState.Is6Enabled = true;
            ButtonState.Is7Enabled = true;
            ButtonState.Is8Enabled = true;
            ButtonState.Is9Enabled = true;
        }

        private void EnableOctButtons()
        {
            ButtonState.IsAEnabled = false;
            ButtonState.IsBEnabled = false;
            ButtonState.IsCEnabled = false;
            ButtonState.IsDEnabled = false;
            ButtonState.IsEEnabled = false;
            ButtonState.IsFEnabled = false;
            ButtonState.Is0Enabled = true;
            ButtonState.Is1Enabled = true;
            ButtonState.Is2Enabled = true;
            ButtonState.Is3Enabled = true;
            ButtonState.Is4Enabled = true;
            ButtonState.Is5Enabled = true;
            ButtonState.Is6Enabled = true;
            ButtonState.Is7Enabled = true;
            ButtonState.Is8Enabled = true;
            ButtonState.Is9Enabled = false;
        }

        private void EnableBinButtons()
        {
            ButtonState.Is0Enabled = true;
            ButtonState.Is1Enabled = true;
            ButtonState.Is2Enabled = false;
            ButtonState.Is3Enabled = false;
            ButtonState.Is4Enabled = false;
            ButtonState.Is5Enabled = false;
            ButtonState.Is6Enabled = false;
            ButtonState.Is7Enabled = false;
            ButtonState.Is8Enabled = false;
            ButtonState.Is9Enabled = false;
            ButtonState.IsAEnabled = false;
            ButtonState.IsBEnabled = false;
            ButtonState.IsCEnabled = false;
            ButtonState.IsDEnabled = false;
            ButtonState.IsEEnabled = false;
            ButtonState.IsFEnabled = false;
        }
    }
}
