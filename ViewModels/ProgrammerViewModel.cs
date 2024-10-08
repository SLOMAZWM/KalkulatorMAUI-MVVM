﻿using KalkulatorMAUI_MVVM.Models;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using System.Collections.Generic;
using System.Linq;
using System;
using System.Windows.Input;
using KalkulatorMAUI_MVVM.Enums;

namespace KalkulatorMAUI_MVVM.ViewModels
{
    public partial class ProgrammerViewModel : ObservableObject
    {
        private const long MaxValue = long.MaxValue;
        private const long MinValue = long.MinValue;

        private bool _isOperationSet = false;
        private bool _isAfterCalculation = false;
        private bool _isDisplayValueUserModified = false;
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
        private bool _isQwordChecked = true;
        [ObservableProperty]
        private bool _isDwordChecked = false;
        [ObservableProperty]
        private bool _isWordChecked = false;
        [ObservableProperty]
        private bool _isByteChecked = false;
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
        [ObservableProperty]
        private PageViewModel _pageViewModel;
        [ObservableProperty]
        private int _openParenthesisCount = 0;

        public ProgrammerViewModel(PageViewModel pageViewModel)
        {
            AvailableNumberSystems = Enum.GetValues(typeof(NumberSystem)).Cast<NumberSystem>().ToList();
            BitShiftOperations = Enum.GetValues(typeof(BitShiftOperation)).Cast<BitShiftOperation>().ToList();
            BitOperations = new List<string> { "NOT", "AND", "OR", "XOR", "NAND", "NOR" };
            CurrentButtonState = new ButtonState();
            CurrentNumberSystem = NumberSystem.DEC;
            _previousNumberSystem = CurrentNumberSystem;
            PageViewModel = pageViewModel;

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
                    _isDisplayValueUserModified = false;
                    break;
                case AmountOfBits.Dword:
                    EnableButtonsUpTo(32);

                    QWordAdditionalInformationIsVisible = false;
                    DWordAdditionalInformationIsVisible = true;
                    WordAdditionalInformationIsVisible = true;

                    Display = "0";
                    _isDisplayValueUserModified = false;
                    break;
                case AmountOfBits.Word:
                    EnableButtonsUpTo(16);

                    QWordAdditionalInformationIsVisible = false;
                    DWordAdditionalInformationIsVisible = false;
                    WordAdditionalInformationIsVisible = true;

                    Display = "0";
                    _isDisplayValueUserModified = false;
                    break;
                case AmountOfBits.Byte:
                    EnableButtonsUpTo(8);

                    QWordAdditionalInformationIsVisible = false;
                    DWordAdditionalInformationIsVisible = false;
                    WordAdditionalInformationIsVisible = false;

                    Display = "0";
                    _isDisplayValueUserModified = false;
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
            if (operation == "NOT")
            {
                PerformNotOperation();
                return;
            }
            else
            {
                Operation = operation;
                FirstNumber = Display;
                _isOperationSet = true;
                Display = "0";
            }
            
        }

        private long PerformBitwiseOperation(long firstNumber, long secondNumber, string operation)
        {
            return operation switch
            {
                "AND" => firstNumber & secondNumber,
                "OR" => firstNumber | secondNumber,
                "XOR" => firstNumber ^ secondNumber,
                "NAND" => ~(firstNumber & secondNumber),
                "NOR" => ~(firstNumber | secondNumber),
                _ => throw new InvalidOperationException("Nieznane działanie bitowe!"),
            };
        }

        private void PerformNotOperation()
        {
            try
            {
                long currentValue = ConvertToDecimalFromSelectedBase(Display.Replace(" ", ""), CurrentNumberSystem);
                long result = ~currentValue;
                Display = NumberFormatter.FormatDisplay(ConvertFromDecimalToSelectedBase(result, CurrentNumberSystem), CurrentNumberSystem);
                OnPropertyChanged(nameof(Display));

                PageViewModel.HistoryOperations.Insert(0, new HistoryOperation
                {
                    Operation = $"NOT {Display}",
                    Result = Display
                });

                _isAfterCalculation = true;
            }
            catch (Exception ex)
            {
                Display = $"BŁĄD: {ex.Message}";
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
            catch (Exception ex)
            {
                Display = $"Błąd: {ex.Message}";
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
                _isDisplayValueUserModified = false;
            }
        }

        [RelayCommand]
        private void SetOperation(string operation)
        {
            if (!_isAfterCalculation)
            {
                if (!string.IsNullOrEmpty(LastOperation) && LastOperation.Last() == ')')
                {
                    LastOperation += operation;
                }
                else
                {
                    if (Display != "0" || _isDisplayValueUserModified)
                    {
                        FirstNumber = Display;
                        LastOperation += Display + operation;
                    }
                    else if (_isDisplayValueUserModified)
                    {
                        LastOperation += Display + operation;
                    }
                    else
                    {
                        LastOperation += operation;
                    }
                }
                Operation = operation;
                Display = "0";
                _isOperationSet = true;
            }
            else
            {
                LastOperation = Display + operation;
                _isAfterCalculation = false;
                Operation = operation;
                Display = "0";
            }

            SelectedBitOperation = null;
        }


        [RelayCommand]
        private void EnterDigitOrCharacter(string sign)
        {
            if (!_isAfterCalculation)
            {
                if (Display == "0" && sign != "0")
                {
                    Display = sign;
                }
                else if (Display == "0" && sign == "0" && _isDisplayValueUserModified)
                {
                    Display = "0";
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
                        Display = displayWithoutSpaces;
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
                LastOperation = string.Empty;
                _isAfterCalculation = false;
            }

            Display = NumberFormatter.FormatDisplay(Display, CurrentNumberSystem);
            _isDisplayValueUserModified = true;
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
            try
            {
                if (_isOperationSet)
                {
                    SecondNumber = Display;
                    LastOperation = $"{FirstNumber} {Operation} {SecondNumber}";

                    if (Operation == "AND" || Operation == "OR" || Operation == "XOR" || Operation == "NAND" || Operation == "NOR")
                    {
                        long firstNumber = ConvertToDecimalFromSelectedBase(FirstNumber, CurrentNumberSystem);
                        long secondNumber = ConvertToDecimalFromSelectedBase(SecondNumber, CurrentNumberSystem);
                        long result = PerformBitwiseOperation(firstNumber, secondNumber, Operation);

                        string formattedResult = NumberFormatter.FormatDisplay(ConvertFromDecimalToSelectedBase(result, CurrentNumberSystem), CurrentNumberSystem);
                        Display = formattedResult;

                        PageViewModel.HistoryOperations.Insert(0, new HistoryOperation
                        {
                            Operation = LastOperation,
                            Result = Display
                        });

                        FirstNumber = ConvertFromDecimalToSelectedBase(result, CurrentNumberSystem);
                        LastOperation += $" = {Display}";
                        _isAfterCalculation = true;
                        _isOperationSet = false;
                    }
                    else
                    {
                        string expression = ConvertExpressionToDecimal(LastOperation);

                        try
                        {
                            System.Data.DataTable table = new System.Data.DataTable();
                            object result = table.Compute(expression, string.Empty);

                            if (result is double || result is float || result is decimal || result is int || result is long)
                            {
                                long answer = Convert.ToInt64(result);
                                if (answer > MaxValue || answer < MinValue)
                                {
                                    Display = "Przekroczono maksymalną wartość";
                                    return;
                                }

                                string formattedResult = NumberFormatter.FormatDisplay(ConvertFromDecimalToSelectedBase(answer, CurrentNumberSystem), CurrentNumberSystem);
                                Display = formattedResult;

                                PageViewModel.HistoryOperations.Insert(0, new HistoryOperation
                                {
                                    Operation = LastOperation,
                                    Result = Display
                                });

                                FirstNumber = ConvertFromDecimalToSelectedBase(answer, CurrentNumberSystem);
                                LastOperation += $" = {formattedResult}";
                                _isAfterCalculation = true;
                            }
                            else
                            {
                                Display = "BŁĄD pozycyjny";
                            }
                        }
                        catch (Exception ex)
                        {
                            Display = $"BŁĄD: {ex.Message}";
                        }
                    }
                }
            }
            catch (OverflowException)
            {
                Display = "Przekroczono maksymalną wartość";
            }
            catch (Exception ex)
            {
                Display = $"BŁĄD: {ex.Message}";
            }

            _isAfterCalculation = true;
        }




        private string ConvertExpressionToDecimal(string expression)
        {
            var regex = new System.Text.RegularExpressions.Regex(@"([0-9A-Fa-f]+)|[\+\-\*/\(\)]");

            var convertedExpression = regex.Replace(expression, match =>
            {
                if ("+-*/()".Contains(match.Value))
                {
                    return match.Value;
                }

                long decimalValue = ConvertToDecimalFromSelectedBase(match.Value, CurrentNumberSystem);
                return decimalValue.ToString();
            });

            return convertedExpression;
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
            catch (Exception ex)
            {
                Display = $"BŁĄD: {ex.Message}";
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
            catch (Exception ex)
            {
                Display = $"BŁĄD: {ex.Message}";
            }
        }


        [RelayCommand]
        private void ClearDisplay()
        {
            Display = NumberFormatter.FormatDisplay("0", CurrentNumberSystem);
            LastOperation = string.Empty;
            Operation = string.Empty;
            _isOperationSet = false;
            _isAfterCalculation = false;
            FirstNumber = "0";
            SecondNumber = "0";
            _isDisplayValueUserModified = false;
        }

        private void ConvertAndDisplay()
        {
            try
            {
                long decimalValue = ConvertToDecimalFromSelectedBase(Display.Replace(" ", ""), _previousNumberSystem);
                Console.WriteLine($"Konwertowanie wartości: {decimalValue} z {_previousNumberSystem} na {CurrentNumberSystem}");
                if (Math.Abs(decimalValue) > MaxValue)
                {
                    Display = "Przekroczono maksymalną wartość";
                    return;
                }
                Display = NumberFormatter.FormatDisplay(ConvertFromDecimalToSelectedBase(decimalValue, CurrentNumberSystem), CurrentNumberSystem);
            }
            catch (Exception ex)
            {
                Display = $"Błąd: {ex.Message}";
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

        [RelayCommand]
        private void AddOpenParenthesis()
        {
            if (Display == "0" && string.IsNullOrEmpty(LastOperation))
            {
                LastOperation += "(";
            }
            else
            {
                if (LastOperation.EndsWith(")") || (!string.IsNullOrEmpty(LastOperation) && char.IsDigit(LastOperation.Last())))
                {
                    LastOperation += "*(";
                }
                else if (Display != "0" && _isDisplayValueUserModified)
                {
                    LastOperation += Display + "*(";
                    Display = "0";
                    _isDisplayValueUserModified = false;
                }
                else
                {
                    LastOperation += "(";
                }
            }
            _isDisplayValueUserModified = false;
            OpenParenthesisCount++;
        }

        [RelayCommand]
        private void AddCloseParenthesis()
        {
            if (OpenParenthesisCount > 0)
            {
                if (Display == "0" && !_isDisplayValueUserModified)
                {
                    LastOperation += ")";
                }
                else
                {
                    LastOperation += Display + ")";
                    Display = "0";
                    _isDisplayValueUserModified = false;
                }
                OpenParenthesisCount--;
            }
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

