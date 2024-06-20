using KalkulatorMAUI_MVVM.Models;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using System;
using System.Collections.Generic;
using System.Linq;
using static System.Runtime.InteropServices.JavaScript.JSType;

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

        [ObservableProperty]
        private string _firstNumber = "0";

        [ObservableProperty]
        private string _secondNumber = "0";

        [ObservableProperty]
        private string _operation = "";

        private bool _isOperationSet = false;

        private bool _isAfterCalculation = false;

        [ObservableProperty]
        private ButtonState _buttonState;

        [ObservableProperty]
        private List<NumberSystem> _numberSystems;

        private NumberSystem _previousNumberSystem;

        public ProgrammerViewModel()
        {
            NumberSystems = Enum.GetValues(typeof(NumberSystem)).Cast<NumberSystem>().ToList();
            ButtonState = new ButtonState();
            SelectedNumberSystem = NumberSystem.DEC;
            _previousNumberSystem = SelectedNumberSystem;
        }

        [RelayCommand]
        private void SetOperation(string operation)
        {
            if (_isOperationSet == false)
            {
                FirstNumber = Display;
                Operation = operation;
                Display = "0";
                _isOperationSet = true;
            }
            else
            {
                SecondNumber = Display;
                Calculation();
                Operation = operation;
            }
        }

        [RelayCommand]
        private void EnterToDisplay(string sign)
        {
            if (!_isAfterCalculation)
            {
                if (Display == "0")
                {
                    Display = sign;
                }
                else
                {
                    Display += sign;
                }
            }
            else
            {
                Display = sign;
                _isAfterCalculation = false;
            }
        }

        [RelayCommand]
        private void Calculation()
        {
            if (_isOperationSet)
            {
                SecondNumber = Display;
            }

            decimal answer = 0;
            switch (Operation)
            {
                case "+":
                    {
                        answer = Convert.ToDecimal(FirstNumber) + Convert.ToDecimal(SecondNumber);
                        break;
                    }
                case "-":
                    {
                        answer = Convert.ToDecimal(FirstNumber) - Convert.ToDecimal(SecondNumber);
                        break;
                    }
                case "*":
                    {
                        answer = Convert.ToDecimal(FirstNumber) * Convert.ToDecimal(SecondNumber);
                        break;
                    }
                case "/":
                    {
                        if (SecondNumber != "0")
                        {
                            answer = Convert.ToDecimal(FirstNumber) / Convert.ToDecimal(SecondNumber);
                        }
                        else
                        {
                            Display = "Error";
                            return;
                        }
                        break;
                    }
                case "%":
                    {
                        answer = Convert.ToDecimal(FirstNumber) % Convert.ToDecimal(SecondNumber);
                        break;
                    }
                default:
                    {
                        throw new InvalidOperationException("Unknown operation");
                    }
            }

            Display = answer.ToString();
            _isAfterCalculation = true;
            FirstNumber = answer.ToString();
            _isOperationSet = false;
        }


        [RelayCommand]
        private void ClearDisplay()
        {
            Display = "0";
            Operation = string.Empty;
            _isOperationSet = false;
            _isAfterCalculation = false;
            FirstNumber = "0";
            SecondNumber = "0";
        }

        partial void OnSelectedNumberSystemChanged(NumberSystem value)
        {
            ConvertAndDisplayValue();
            _previousNumberSystem = SelectedNumberSystem;
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

        private void ConvertAndDisplayValue()
        {
            try
            {
                int decimalValue = ConvertToDecimal(Display, _previousNumberSystem);
                Display = ConvertFromDecimal(decimalValue, SelectedNumberSystem);
            }
            catch
            {
                Display = "Błąd";
            }
        }

        private int ConvertToDecimal(string value, NumberSystem system)
        {
            return system switch
            {
                NumberSystem.HEX => Convert.ToInt32(value, 16),
                NumberSystem.DEC => int.Parse(value),
                NumberSystem.OCT => Convert.ToInt32(value, 8),
                NumberSystem.BIN => Convert.ToInt32(value, 2),
                _ => throw new InvalidOperationException("Unknown number system"),
            };
        }

        private string ConvertFromDecimal(int value, NumberSystem system)
        {
            return system switch
            {
                NumberSystem.HEX => value.ToString("X"),
                NumberSystem.DEC => value.ToString(),
                NumberSystem.OCT => Convert.ToString(value, 8),
                NumberSystem.BIN => Convert.ToString(value, 2),
                _ => throw new InvalidOperationException("Unknown number system"),
            };
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
            ButtonState.Is8Enabled = false;
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
