using KalkulatorMAUI_MVVM.Models;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using System.Collections.Generic;
using System.Linq;
using System;
using System.Windows.Input;

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
        Arytmetyczne,
        Logiczne,
        Cykliczne,
    }

    public enum AmountOfBits
    {
        Qword,
        Dword,
        Word,
        Byte,
    }

    public partial class ProgrammerViewModel : ObservableObject
    {
        private const long MaxValue = long.MaxValue;
        private const long MinValue = long.MinValue;

        private bool _isOperationSet = false;

        private bool _isAfterCalculation = false;

        private NumberSystem _previousNumberSystem;

        private AmountOfBits _selectedAmountOfBits;

        [ObservableProperty]
        private bool _standardInputCalculatorIsVisible = true;

        [ObservableProperty]
        private bool _qwordInputCalculatorIsVisible = false;

        [ObservableProperty]
        private BitShiftOperation _selectedBitShiftOperation;

        [ObservableProperty]
        private List<BitShiftOperation> _bitShiftOperations;

        [ObservableProperty]
        private NumberSystem _currentNumberSystem;

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

        [ObservableProperty]
        private string? _selectedBitOperation;

        [ObservableProperty]
        private ButtonState _currentButtonState;

        [ObservableProperty]
        private List<NumberSystem> _availableNumberSystems;

        [ObservableProperty]
        private List<string> _bitOperations;

        [ObservableProperty]
        private bool isQwordChecked = true;

        [ObservableProperty]
        private bool isDwordChecked = false;

        [ObservableProperty]
        private bool isWordChecked = false;

        [ObservableProperty]
        private bool isByteChecked = false;

        [ObservableProperty]
        private bool[] _bitButtonState = new bool[64];

        [ObservableProperty]
        private bool[] _bitButtonIsEnabled = new bool[64];

        [ObservableProperty]
        private bool _wordAdditionalInformationIsVisible = true;

        [ObservableProperty]
        private bool _dWordAdditionalInformationIsVisible = true;

        [ObservableProperty]
        private bool _qWordAdditionalInformationIsVisible = true;

        public ProgrammerViewModel()
        {
            AvailableNumberSystems = Enum.GetValues(typeof(NumberSystem)).Cast<NumberSystem>().ToList();
            BitShiftOperations = Enum.GetValues(typeof(BitShiftOperation)).Cast<BitShiftOperation>().ToList();
            BitOperations = new List<string> { "NOT", "AND", "OR", "XOR", "NAND", "NOR" };
            CurrentButtonState = new ButtonState();
            CurrentNumberSystem = NumberSystem.DEC;
            _previousNumberSystem = CurrentNumberSystem;

            InitializeProgrammerCalculator();
        }

        public void InitializeProgrammerCalculator()
        {
            PropertyChanged += (sender, e) =>
            {
                if (e.PropertyName == nameof(Display))
                {
                    UpdateBitButtonState(Display);
                }
            };

            PropertyChanged += (sender, e) =>
            {
                if (e.PropertyName == nameof(IsQwordChecked) && IsQwordChecked)
                {
                    UpdateAmountOfBits(AmountOfBits.Qword);
                }
                else if (e.PropertyName == nameof(IsDwordChecked) && IsDwordChecked)
                {
                    UpdateAmountOfBits(AmountOfBits.Dword);
                }
                else if (e.PropertyName == nameof(IsWordChecked) && IsWordChecked)
                {
                    UpdateAmountOfBits(AmountOfBits.Word);
                }
                else if (e.PropertyName == nameof(IsByteChecked) && IsByteChecked)
                {
                    UpdateAmountOfBits(AmountOfBits.Byte);
                }
            };

            UpdateAmountOfBits(AmountOfBits.Qword);
        }

        private void UpdateAmountOfBits(AmountOfBits amountOfBits)
        {
            _selectedAmountOfBits = amountOfBits;
            ChangeButtonsStateFromAmountOfBitsSelected(_selectedAmountOfBits);
        }

        private void UpdateBitButtonState(string displayValue)
        {
            try
            {
                long currentValue = ConvertToDecimalFromSelectedBase(displayValue.Replace(" ", ""), CurrentNumberSystem);
                for (int i = 0; i < 64; i++)
                {
                    BitButtonState[i] = (currentValue & (1L << i)) != 0;
                }
                OnPropertyChanged(nameof(BitButtonState));
            }
            catch
            {
                Display = "BŁĄD";
            }
        }

        partial void OnSelectedBitOperationChanged(string? value)
        {
            if (!string.IsNullOrEmpty(value))
            {
                SetBitOperation(value);
            }
        }

        private bool CompareButtonStateWithPositionButton(int bit)
        {
            bool BitButtonActivated = BitButtonState[bit];
            BitButtonState[bit] = !BitButtonState[bit];

            return BitButtonActivated;
        }

        [RelayCommand]
        private void ToggleBit(string bitPositionButton)
        {
            int bitPosition = Convert.ToInt32(bitPositionButton);
            if (bitPosition < 0 || bitPosition > 63)
            {
                Display = "BŁĄD";
                return;
            }

            bool bitActivated = CompareButtonStateWithPositionButton(bitPosition);

            try
            {
                long currentValue = ConvertToDecimalFromSelectedBase(Display.Replace(" ", ""), CurrentNumberSystem);

                if (bitActivated)
                {
                    currentValue &= ~(1L << bitPosition);
                }
                else
                {
                    currentValue |= (1L << bitPosition);
                }

                Display = NumberFormatter.FormatDisplay(ConvertFromDecimalToSelectedBase(currentValue, CurrentNumberSystem), CurrentNumberSystem);
                OnPropertyChanged(nameof(BitButtonState));
            }
            catch
            {
                Display = "BŁĄD";
            }
        }

        [RelayCommand]
        private void ChangeButtonsStateFromAmountOfBitsSelected(AmountOfBits selectedAmountOfBits)
        {
            switch (selectedAmountOfBits)
            {
                case AmountOfBits.Qword:
                    EnableAllButtons();

                    QWordAdditionalInformationIsVisible = true;
                    DWordAdditionalInformationIsVisible = true;
                    WordAdditionalInformationIsVisible = true;

                    Display = "0";
                    break;
                case AmountOfBits.Dword:
                    EnableButtonsUpTo(32);

                    QWordAdditionalInformationIsVisible = false;
                    DWordAdditionalInformationIsVisible = true;
                    WordAdditionalInformationIsVisible = true;

                    Display = "0";
                    break;
                case AmountOfBits.Word:
                    EnableButtonsUpTo(16);

                    QWordAdditionalInformationIsVisible = false;
                    DWordAdditionalInformationIsVisible = false;
                    WordAdditionalInformationIsVisible = true;

                    Display = "0";
                    break;
                case AmountOfBits.Byte:
                    EnableButtonsUpTo(8);

                    QWordAdditionalInformationIsVisible = false;
                    DWordAdditionalInformationIsVisible = false;
                    WordAdditionalInformationIsVisible = false;

                    Display = "0";
                    break;
            }
        }

        private void EnableAllButtons()
        {
            for (int i = 0; i < 64; i++)
            {
                BitButtonIsEnabled[i] = true;
            }
            UpdateButtonsState();
        }

        private void EnableButtonsUpTo(int count)
        {
            for (int i = 0; i < count; i++)
            {
                BitButtonIsEnabled[i] = true;
            }
            for (int i = count; i < 64; i++)
            {
                BitButtonIsEnabled[i] = false;
            }
            UpdateButtonsState();
        }

        private void UpdateButtonsState()
        {
            OnPropertyChanged(nameof(BitButtonIsEnabled));
        }


        [RelayCommand]
        private void StandardInputCalculatorVisibilityTrue()
        {
            StandardInputCalculatorIsVisible = true;
            QwordInputCalculatorIsVisible = false;
            CurrentNumberSystem = _previousNumberSystem;
        }

        [RelayCommand]
        private void QwordInputCalculatorVisibilityTrue()
        {
            StandardInputCalculatorIsVisible = false;
            QwordInputCalculatorIsVisible = true;
            CurrentNumberSystem = NumberSystem.DEC;
        }

        [RelayCommand]
        private void SetBitOperation(string operation)
        {
            switch (operation)
            {
                case "NOT":
                    PerformNotOperation();
                    break;
                case "AND":
                    SetOperation("AND");
                    break;
                case "OR":
                    SetOperation("OR");
                    break;
                case "XOR":
                    SetOperation("XOR");
                    break;
                case "NAND":
                    SetOperation("NAND");
                    break;
                case "NOR":
                    SetOperation("NOR");
                    break;
            }
        }

        private void PerformNotOperation()
        {
            try
            {
                long currentValue = ConvertToDecimalFromSelectedBase(Display, CurrentNumberSystem);
                currentValue = ~currentValue;
                Display = NumberFormatter.FormatDisplay(ConvertFromDecimalToSelectedBase(currentValue, CurrentNumberSystem), CurrentNumberSystem);
            }
            catch
            {
                Display = "BŁĄD";
            }
        }

        partial void OnCurrentNumberSystemChanged(NumberSystem value)
        {
            ConvertAndDisplay();
            _previousNumberSystem = CurrentNumberSystem;
            switch (CurrentNumberSystem)
            {
                case NumberSystem.HEX:
                    EnableHexadecimalButtons();
                    break;
                case NumberSystem.DEC:
                    EnableDecimalButtons();
                    break;
                case NumberSystem.OCT:
                    EnableOctalButtons();
                    break;
                case NumberSystem.BIN:
                    EnableBinaryButtons();
                    break;
            }
        }

        [RelayCommand]
        private void ToggleSign()
        {
            try
            {
                long currentValue = ConvertToDecimalFromSelectedBase(Display, CurrentNumberSystem);
                currentValue = -currentValue;

                Display = NumberFormatter.FormatDisplay(ConvertFromDecimalToSelectedBase(currentValue, CurrentNumberSystem), CurrentNumberSystem);
            }
            catch
            {
                Display = "Błąd";
            }
        }

        [RelayCommand]
        private void DeleteSign()
        {
            if (Display.Length > 0)
            {
                Display = Display.Remove(Display.Length - 1);
            }

            if (Display.Length == 0)
            {
                Display = "0";
            }
        }

        [RelayCommand]
        private void SetOperation(string operation)
        {
            if (!_isOperationSet)
            {
                FirstNumber = Display;
                Operation = operation;
                Display = "0";
                _isOperationSet = true;
                LastOperation = FirstNumber + Operation;

                if (Operation == "leftArrow" && SelectedBitShiftOperation == BitShiftOperation.Cykliczne)
                {
                    PerformLeftShiftOperation();
                    _isOperationSet = false;
                }
                else if (Operation == "rightArrow" && SelectedBitShiftOperation == BitShiftOperation.Cykliczne)
                {
                    PerformRightShiftOperation();
                    _isOperationSet = false;
                }
            }
            else
            {
                SecondNumber = Display;
                LastOperation = FirstNumber + Operation + SecondNumber;
                PerformCalculation();
                Operation = operation;
            }

            SelectedBitOperation = null;
        }

        [RelayCommand]
        private void EnterDigitOrCharacter(string sign)
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
                        long currentValue = Convert.ToInt64(displayWithoutSpaces, GetBaseFromNumberSystem(CurrentNumberSystem));
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

            Display = NumberFormatter.FormatDisplay(Display, CurrentNumberSystem);
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
        private void PerformCalculation()
        {
            if (_isOperationSet)
            {
                SecondNumber = Display;
            }

            try
            {
                long firstNumberInDecimal = ConvertToDecimalFromSelectedBase(FirstNumber.Replace(" ", ""), CurrentNumberSystem);

                long secondNumberInDecimal = ConvertToDecimalFromSelectedBase(SecondNumber.Replace(" ", ""), CurrentNumberSystem);

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
                    case "leftArrow":
                        PerformLeftShiftOperation();
                        return;
                    case "rightArrow":
                        PerformRightShiftOperation();
                        return;
                    case "AND":
                        answer = firstNumberInDecimal & secondNumberInDecimal;
                        break;
                    case "OR":
                        answer = firstNumberInDecimal | secondNumberInDecimal;
                        break;
                    case "XOR":
                        answer = firstNumberInDecimal ^ secondNumberInDecimal;
                        break;
                    case "NAND":
                        answer = ~(firstNumberInDecimal & secondNumberInDecimal);
                        break;
                    case "NOR":
                        answer = ~(firstNumberInDecimal | secondNumberInDecimal);
                        break;
                    default:
                        throw new InvalidOperationException("Nieznana operacja");
                }

                if (answer > MaxValue || answer < MinValue)
                {
                    Display = "Przekroczono maksymalną wartość";
                    return;
                }

                Display = NumberFormatter.FormatDisplay(ConvertFromDecimalToSelectedBase(answer, CurrentNumberSystem), CurrentNumberSystem);
                _isAfterCalculation = true;

                FirstNumber = ConvertFromDecimalToSelectedBase(answer, CurrentNumberSystem);
                _isOperationSet = false;
                LastOperation = FirstNumber + Operation + SecondNumber + "=" + answer.ToString();
            }
            catch (OverflowException)
            {
                Display = "Przekroczono maksymalną wartość";
            }
            catch (Exception ex)
            {
                Display = $"BŁĄD: {ex.Message}";
            }
        }

        private void PerformRightShiftOperation()
        {
            try
            {
                long currentValue = ConvertToDecimalFromSelectedBase(FirstNumber, CurrentNumberSystem);
                int shiftAmount = SelectedBitShiftOperation == BitShiftOperation.Cykliczne ? 1 : Convert.ToInt32(ConvertToDecimalFromSelectedBase(SecondNumber, CurrentNumberSystem));

                switch (SelectedBitShiftOperation)
                {
                    case BitShiftOperation.Arytmetyczne:
                        currentValue >>= shiftAmount;
                        break;
                    case BitShiftOperation.Logiczne:
                        currentValue = (long)((ulong)currentValue >> shiftAmount);
                        break;
                    case BitShiftOperation.Cykliczne:
                        int size = sizeof(long) * 8;
                        currentValue = (currentValue >> shiftAmount) | ((currentValue & ((1L << shiftAmount) - 1)) << (size - shiftAmount));
                        break;
                }

                Display = NumberFormatter.FormatDisplay(ConvertFromDecimalToSelectedBase(currentValue, CurrentNumberSystem), CurrentNumberSystem);
            }
            catch
            {
                Display = "BŁĄD";
            }
        }

        private void PerformLeftShiftOperation()
        {
            try
            {
                long currentValue = ConvertToDecimalFromSelectedBase(FirstNumber, CurrentNumberSystem);
                int shiftAmount = SelectedBitShiftOperation == BitShiftOperation.Cykliczne ? 1 : Convert.ToInt32(ConvertToDecimalFromSelectedBase(SecondNumber, CurrentNumberSystem));

                switch (SelectedBitShiftOperation)
                {
                    case BitShiftOperation.Arytmetyczne:
                        currentValue <<= shiftAmount;
                        break;
                    case BitShiftOperation.Logiczne:
                        currentValue = (long)((ulong)currentValue << shiftAmount);
                        break;
                    case BitShiftOperation.Cykliczne:
                        int size = sizeof(long) * 8;
                        currentValue = (currentValue << shiftAmount) | ((long)((ulong)currentValue >> (size - shiftAmount)));
                        break;
                }

                Display = NumberFormatter.FormatDisplay(ConvertFromDecimalToSelectedBase(currentValue, CurrentNumberSystem), CurrentNumberSystem);
            }
            catch
            {
                Display = "BŁĄD";
            }
        }

        [RelayCommand]
        private void ClearDisplay()
        {
            Display = NumberFormatter.FormatDisplay("0", CurrentNumberSystem);
            Operation = string.Empty;
            _isOperationSet = false;
            _isAfterCalculation = false;
            FirstNumber = "0";
            SecondNumber = "0";
        }

        private void ConvertAndDisplay()
        {
            try
            {
                long decimalValue = ConvertToDecimalFromSelectedBase(Display.Replace(" ", ""), _previousNumberSystem);
                if (Math.Abs(decimalValue) > MaxValue)
                {
                    Display = "Przekroczono maksymalną wartość";
                    return;
                }
                Display = NumberFormatter.FormatDisplay(ConvertFromDecimalToSelectedBase(decimalValue, CurrentNumberSystem), CurrentNumberSystem);
            }
            catch
            {
                Display = "Błąd";
            }
        }

        private long ConvertToDecimalFromSelectedBase(string value, NumberSystem system)
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

        private string ConvertFromDecimalToSelectedBase(long value, NumberSystem system)
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

        private void EnableHexadecimalButtons()
        {
            CurrentButtonState.IsAEnabled = true;
            CurrentButtonState.IsBEnabled = true;
            CurrentButtonState.IsCEnabled = true;
            CurrentButtonState.IsDEnabled = true;
            CurrentButtonState.IsEEnabled = true;
            CurrentButtonState.IsFEnabled = true;
            CurrentButtonState.Is0Enabled = true;
            CurrentButtonState.Is1Enabled = true;
            CurrentButtonState.Is2Enabled = true;
            CurrentButtonState.Is3Enabled = true;
            CurrentButtonState.Is4Enabled = true;
            CurrentButtonState.Is5Enabled = true;
            CurrentButtonState.Is6Enabled = true;
            CurrentButtonState.Is7Enabled = true;
            CurrentButtonState.Is8Enabled = true;
            CurrentButtonState.Is9Enabled = true;
        }

        private void EnableDecimalButtons()
        {
            CurrentButtonState.IsAEnabled = false;
            CurrentButtonState.IsBEnabled = false;
            CurrentButtonState.IsCEnabled = false;
            CurrentButtonState.IsDEnabled = false;
            CurrentButtonState.IsEEnabled = false;
            CurrentButtonState.IsFEnabled = false;
            CurrentButtonState.Is0Enabled = true;
            CurrentButtonState.Is1Enabled = true;
            CurrentButtonState.Is2Enabled = true;
            CurrentButtonState.Is3Enabled = true;
            CurrentButtonState.Is4Enabled = true;
            CurrentButtonState.Is5Enabled = true;
            CurrentButtonState.Is6Enabled = true;
            CurrentButtonState.Is7Enabled = true;
            CurrentButtonState.Is8Enabled = true;
            CurrentButtonState.Is9Enabled = true;
        }

        private void EnableOctalButtons()
        {
            CurrentButtonState.IsAEnabled = false;
            CurrentButtonState.IsBEnabled = false;
            CurrentButtonState.IsCEnabled = false;
            CurrentButtonState.IsDEnabled = false;
            CurrentButtonState.IsEEnabled = false;
            CurrentButtonState.IsFEnabled = false;
            CurrentButtonState.Is0Enabled = true;
            CurrentButtonState.Is1Enabled = true;
            CurrentButtonState.Is2Enabled = true;
            CurrentButtonState.Is3Enabled = true;
            CurrentButtonState.Is4Enabled = true;
            CurrentButtonState.Is5Enabled = true;
            CurrentButtonState.Is6Enabled = true;
            CurrentButtonState.Is7Enabled = true;
            CurrentButtonState.Is8Enabled = false;
            CurrentButtonState.Is9Enabled = false;
        }

        private void EnableBinaryButtons()
        {
            CurrentButtonState.Is0Enabled = true;
            CurrentButtonState.Is1Enabled = true;
            CurrentButtonState.Is2Enabled = false;
            CurrentButtonState.Is3Enabled = false;
            CurrentButtonState.Is4Enabled = false;
            CurrentButtonState.Is5Enabled = false;
            CurrentButtonState.Is6Enabled = false;
            CurrentButtonState.Is7Enabled = false;
            CurrentButtonState.Is8Enabled = false;
            CurrentButtonState.Is9Enabled = false;
            CurrentButtonState.IsAEnabled = false;
            CurrentButtonState.IsBEnabled = false;
            CurrentButtonState.IsCEnabled = false;
            CurrentButtonState.IsDEnabled = false;
            CurrentButtonState.IsEEnabled = false;
            CurrentButtonState.IsFEnabled = false;
        }
    }
}
