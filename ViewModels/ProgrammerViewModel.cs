using KalkulatorMAUI_MVVM.Models;

namespace KalkulatorMAUI_MVVM.ViewModels
{
    public enum NumberSystem
    {
        HEX,
        DEC,
        OCT,
        BIN,
    }

    public enum BitShiftOperation
    {
        ArithmeticShift,
        LogicalShift,
        CircularShift,
        CircularShiftThroughCarry,
    }

    public partial class ProgrammerViewModel : ObservableObject
    {
        private const long MaxValue = long.MaxValue;
        private const long MinValue = long.MinValue;

        [ObservableProperty]
        private BitShiftOperation _selectedBitShiftMode;

        [ObservableProperty]
        private List<BitShiftOperation> _bitShiftModes;

        [ObservableProperty]
        private NumberSystem _selectedNumberSystem;

        [ObservableProperty]
        private string _lastOperation = string.Empty;

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
            BitShiftModes = Enum.GetValues(typeof(BitShiftOperation)).Cast<BitShiftOperation>().ToList();
            ButtonState = new ButtonState();
            SelectedNumberSystem = NumberSystem.DEC;
            _previousNumberSystem = SelectedNumberSystem;
        }

        [RelayCommand]
        private void ToogleSign()
        {
            try
            {
                long currentValue = ConvertToDecimal(Display, SelectedNumberSystem);
                currentValue = -currentValue;

                Display = NumberFormatter.FormatDisplay(ConvertFromDecimal(currentValue, SelectedNumberSystem), SelectedNumberSystem);
            }
            catch
            {
                Display = "Błąd";
            }
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
                LastOperation = FirstNumber + Operation;
            }
            else
            {
                SecondNumber = Display;
                LastOperation = FirstNumber + Operation + SecondNumber;
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
                    string displayWithoutSpaces = Display.Replace(" ", "") + sign;
                    try
                    {
                        long currentValue = Convert.ToInt64(displayWithoutSpaces, GetBaseFromNumberSystem(SelectedNumberSystem));
                        if (currentValue > MaxValue)
                        {
                            return;
                        }
                        Display += sign;
                    }
                    catch
                    {
                        return;
                    }
                }
            }
            else
            {
                Display = sign;
                _isAfterCalculation = false;
            }

            Display = NumberFormatter.FormatDisplay(Display, SelectedNumberSystem);
        }

        private int GetBaseFromNumberSystem(NumberSystem system)
        {
            return system switch
            {
                NumberSystem.HEX => 16,
                NumberSystem.DEC => 10,
                NumberSystem.OCT => 8,
                NumberSystem.BIN => 2,
                _ => throw new InvalidOperationException("Nieznany system liczbowy!"),
            };
        }

        [RelayCommand]
        private void Calculation()
        {
            if (_isOperationSet)
            {
                SecondNumber = Display;
            }

            try
            {
                long firstNumberInDecimal = ConvertToDecimal(FirstNumber.Replace(" ", ""), SelectedNumberSystem);
                long secondNumberInDecimal = ConvertToDecimal(SecondNumber.Replace(" ", ""), SelectedNumberSystem);

                long answer = 0;
                switch (Operation)
                {
                    case "+":
                        answer = firstNumberInDecimal + secondNumberInDecimal;
                        break;
                    case "-":
                        answer = firstNumberInDecimal - secondNumberInDecimal;
                        break;
                    case "*":
                        answer = firstNumberInDecimal * secondNumberInDecimal;
                        break;
                    case "/":
                        if (secondNumberInDecimal != 0)
                        {
                            answer = firstNumberInDecimal / secondNumberInDecimal;
                        }
                        else
                        {
                            Display = "NIE DZIEL PRZEZ ZERO!";
                            return;
                        }
                        break;
                    case "%":
                        answer = firstNumberInDecimal % secondNumberInDecimal;
                        break;
                    default:
                        throw new InvalidOperationException("Nieznana operacja");
                }

                if (answer > MaxValue || answer < MinValue)
                {
                    Display = "Przekroczono maksymalną wartość";
                    return;
                }

                Display = NumberFormatter.FormatDisplay(ConvertFromDecimal(answer, SelectedNumberSystem), SelectedNumberSystem);
                _isAfterCalculation = true;

                FirstNumber = ConvertFromDecimal(answer, SelectedNumberSystem);
                _isOperationSet = false;
                LastOperation = FirstNumber + Operation + SecondNumber + "=" + answer.ToString();
            }
            catch (OverflowException)
            {
                Display = "Przekroczono maksymalną wartość";
            }
        }

        [RelayCommand]
        private void ClearDisplay()
        {
            Display = NumberFormatter.FormatDisplay("0", SelectedNumberSystem);
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
                long decimalValue = ConvertToDecimal(Display.Replace(" ", ""), _previousNumberSystem);
                if (Math.Abs(decimalValue) > MaxValue)
                {
                    Display = "Przekroczono maksymalną wartość";
                    return;
                }
                Display = NumberFormatter.FormatDisplay(ConvertFromDecimal(decimalValue, SelectedNumberSystem), SelectedNumberSystem);
            }
            catch
            {
                Display = "Błąd";
            }
        }

        private long ConvertToDecimal(string value, NumberSystem system)
        {
            value = value.Replace(" ", "");
            return system switch
            {
                NumberSystem.HEX => Convert.ToInt64(value, 16),
                NumberSystem.DEC => long.Parse(value),
                NumberSystem.OCT => Convert.ToInt64(value, 8),
                NumberSystem.BIN => Convert.ToInt64(value, 2),
                _ => throw new InvalidOperationException("Nieznany system numeryczny!"),
            };
        }

        private string ConvertFromDecimal(long value, NumberSystem system)
        {
            return system switch
            {
                NumberSystem.HEX => value.ToString("X"),
                NumberSystem.DEC => value.ToString(),
                NumberSystem.OCT => Convert.ToString(value, 8),
                NumberSystem.BIN => Convert.ToString(value, 2),
                _ => throw new InvalidOperationException("Nieznany system numeryczny!"),
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
