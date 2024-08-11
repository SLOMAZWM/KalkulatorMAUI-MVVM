using KalkulatorMAUI_MVVM.Entities;
using KalkulatorMAUI_MVVM.Models;
using System.Data;

namespace KalkulatorMAUI_MVVM.ViewModels
{
    public partial class CalculatorViewModel : ObservableObject
    {
        [ObservableProperty] private PageViewModel _pageViewModel;

        [ObservableProperty] private string _lastOperation = string.Empty;

        [ObservableProperty] private string _display = "0";

        [ObservableProperty] private bool _isFnSelected = false;

        [ObservableProperty] private bool _isFnUnSelected = true;

        [ObservableProperty] private string _fnBackgroundColor = "#222222";

        [ObservableProperty] private string _fnTextColor = "White";

        [ObservableProperty] private bool _isRadSelected = false;

        private bool _isDisplayValueUserModified = false;

        [ObservableProperty] private string _nameOfRadDegButton = "Rad";

        private bool _isAfterCalculation = false;

        [ObservableProperty] private bool _isMenuVisible = false;

        [ObservableProperty] private bool _isButtonsVisible = true;

        [ObservableProperty] private int _openParenthesisCount = 0;

        [ObservableProperty] private bool _isMemoryVisible = false;

        [ObservableProperty] private MemoryStore _memoryStore;

        private string _currentOperation = string.Empty;
        private double _firstOperand;
        private double _secondOperand;

        public CalculatorViewModel(PageViewModel pageViewModel)
        {
            PageViewModel = pageViewModel;
            MemoryStore = new MemoryStore();
         }

        [RelayCommand]
        private void SetOperation(string operation)
        {
            if (!_isAfterCalculation)
            {
                if (operation == "pow" || operation == "LogBaseY" || operation == "EE" || operation == "root")
                {
                    _currentOperation = operation;
                    _firstOperand = Convert.ToDouble(Display);

                    if (operation == "pow")
                    {
                        LastOperation += Display + "^";
                    }
                    else if (operation == "LogBaseY")
                    {
                        LastOperation += "log" + Display + "(";
                    }
                    else if (operation == "EE")
                    {
                        LastOperation += Display + "E";
                    }
                    else if (operation == "root")
                    {
                        LastOperation += Display + "root";
                    }

                    Display = "0";
                }
                else
                {
                    if (!string.IsNullOrEmpty(LastOperation) && LastOperation.Last() == ')')
                    {
                        LastOperation += operation;
                    }
                    else if (Display != "0" || operation == "(" || operation == ")")
                    {
                        LastOperation += Display + operation;
                        Display = "0";
                    }
                }
            }
            else
            {
                if (operation == "pow" || operation == "LogBaseY" || operation == "EE" || operation == "root")
                {
                    if (operation == "pow")
                    {
                        LastOperation = Display + "^";
                    }
                    else if (operation == "LogBaseY")
                    {
                        LastOperation = "log" + Display + "(";
                    }
                    else if (operation == "EE")
                    {
                        LastOperation = Display + "E";
                    }
                    else if (operation == "root")
                    {
                        LastOperation = Display + "root";
                    }

                    Display = "0";
                }
                else
                {
                    if (!string.IsNullOrEmpty(LastOperation) && LastOperation.Last() == ')')
                    {
                        LastOperation = operation;
                    }
                    else if (Display != "0" || operation == "(" || operation == ")")
                    {
                        LastOperation = Display + operation;
                        Display = "0";
                    }
                }
                
                _currentOperation = operation;
                _firstOperand = Convert.ToDouble(Display);
            }
            
        }


        [RelayCommand]
        private void SetPercent()
        {
            try
            {
                decimal percent = Convert.ToDecimal(Display) / 100;
                Display = percent.ToString();
            }
            catch (FormatException)
            {
                Display = "Error";
            }
        }

        [RelayCommand]
        private void AddNumber(string number)
        {
            if (_isAfterCalculation)
            {
                Display = number;
                _isAfterCalculation = false;
            }
            else
            {
                if (Display == "0")
                {
                    Display = number;
                }
                else
                {
                    Display += number;
                }
            }

            _isDisplayValueUserModified = true;
        }

        [RelayCommand]
        private void ChangeSign()
        {
            if (Display.StartsWith("-"))
            {
                Display = Display.Substring(1);
            }
            else if (Display != "0")
            {
                Display = "-" + Display;
            }
        }

        [RelayCommand]
        private void Clear()
        {
            Display = "0";
            LastOperation = string.Empty;
            _isAfterCalculation = false;
            OpenParenthesisCount = 0;
            _isDisplayValueUserModified = false;
            _currentOperation = string.Empty;
        }

        [RelayCommand]
        private void AddDot()
        {
            if (!Display.Contains("."))
            {
                Display += ".";
            }
        }

        private void AddHistoryToPageViewModel(string Operation, string Answer)
        {
            PageViewModel.HistoryOperations.Insert(0, new HistoryOperation()
            {
                Operation = Operation,
                Result = Answer
            });
        }
        
        [RelayCommand]
        private void Equals()
        {
            try
            {
                if (Display != "0" || string.IsNullOrWhiteSpace(LastOperation))
                {
                    LastOperation += Display;
                }

                if (_currentOperation == "pow")
                {
                    _secondOperand = Convert.ToDouble(Display);
                    Display = Math.Pow(_firstOperand, _secondOperand).ToString();
                    LastOperation = LastOperation.Replace("^" + Display, "^" + _secondOperand.ToString());
                    
                    AddHistoryToPageViewModel(LastOperation, Display);

                }
                else if (_currentOperation == "LogBaseY")
                {
                    _secondOperand = Convert.ToDouble(Display);
                    Display = Math.Log(_firstOperand, _secondOperand).ToString();
                    LastOperation = LastOperation.Replace("log" + _firstOperand + "(" + Display,
                        "log" + _firstOperand + "(" + _secondOperand.ToString());
                    
                    AddHistoryToPageViewModel(LastOperation, Display);

                }
                else if (_currentOperation == "EE")
                {
                    _secondOperand = Convert.ToDouble(Display);
                    Display = (_firstOperand * Math.Pow(10, _secondOperand)).ToString();
                    LastOperation = LastOperation.Replace("E" + Display, "E" + _secondOperand.ToString());
                    
                    AddHistoryToPageViewModel(LastOperation, Display);
                }
                else if (_currentOperation == "root")
                {
                    _secondOperand = Convert.ToDouble(Display);
                    Display = Math.Pow(_firstOperand, 1.0 / _secondOperand).ToString();
                    LastOperation = LastOperation.Replace("root" + Display, "root" + _secondOperand.ToString());

                    AddHistoryToPageViewModel(LastOperation, Display);
                }
                else
                {
                    string expression = LastOperation;

                    DataTable table = new DataTable();

                    table.Columns.Add("expression", typeof(string), expression);
                    DataRow row = table.NewRow();
                    table.Rows.Add(row);

                    var result = row["expression"];

                    Display = Convert.ToString(result);
                    
                    AddHistoryToPageViewModel(expression, result.ToString()!);
                }
                
                
                LastOperation += " = " + Display;
                _isAfterCalculation = true;
                _currentOperation = string.Empty;
            }
            catch (Exception)
            {
                Display = "Error";
            }
        }


        [RelayCommand]
        private void GetSqrt()
        {
            try
            {
                Display = Math.Sqrt(Convert.ToDouble(Display)).ToString();
            }
            catch (FormatException)
            {
                Display = "Error";
            }
        }

        [RelayCommand]
        private void GetRoot3()
        {
            try
            {
                Display = Math.Pow(Convert.ToDouble(Display), 1.0 / 3).ToString();
            }
            catch (FormatException)
            {
                Display = "Error";
            }
        }

        [RelayCommand]
        private void InvertNumber()
        {
            try
            {
                Display = (1 / Convert.ToDouble(Display)).ToString();
            }
            catch (FormatException)
            {
                Display = "Error";
            }
        }

        [RelayCommand]
        private void PowNumber()
        {
            try
            {
                Display = Math.Pow(Convert.ToDouble(Display), 2).ToString();
            }
            catch (FormatException)
            {
                Display = "Error";
            }
        }

        [RelayCommand]
        private void Pow3OfNumber()
        {
            try
            {
                Display = Math.Pow(Convert.ToDouble(Display), 3).ToString();
            }
            catch (FormatException)
            {
                Display = "Error";
            }
        }

        [RelayCommand]
        private void ExpOfX()
        {
            try
            {
                Display = Math.Exp(Convert.ToDouble(Display)).ToString();
            }
            catch (FormatException)
            {
                Display = "Error";
            }
        }

        [RelayCommand]
        private void Pow10OfX()
        {
            try
            {
                Display = Math.Pow(10, Convert.ToDouble(Display)).ToString();
            }
            catch (FormatException)
            {
                Display = "Error";
            }
        }

        [RelayCommand]
        private void GetPiNumber()
        {
            Display = Math.PI.ToString();
        }

        [RelayCommand]
        private void GetENumber()
        {
            Display = Math.E.ToString();
        }

        [RelayCommand]
        private void GetNLog()
        {
            try
            {
                Display = Math.Log(Convert.ToDouble(Display)).ToString();
            }
            catch (FormatException)
            {
                Display = "Error";
            }
        }

        [RelayCommand]
        private void GetN10Log()
        {
            try
            {
                Display = Math.Log10(Convert.ToDouble(Display)).ToString();
            }
            catch (FormatException)
            {
                Display = "Error";
            }
        }

        [RelayCommand]
        private void GetFactorial()
        {
            try
            {
                int number = Convert.ToInt32(Display);
                long factorial = 1;
                for (int i = 2; i <= number; i++)
                {
                    factorial *= i;
                }

                Display = factorial.ToString();
            }
            catch (FormatException)
            {
                Display = "Error";
            }
        }

        private double DegreesToRadians(double degrees)
        {
            return degrees * (Math.PI / 180);
        }

        private double RadiansToDegrees(double radians)
        {
            return radians * (180 / Math.PI);
        }

        [RelayCommand]
        private void GetSin()
        {
            try
            {
                double input = Convert.ToDouble(Display);
                double radians = IsRadSelected ? input : DegreesToRadians(input);
                double sin = Math.Sin(radians);
                Display = sin.ToString();
            }
            catch (FormatException)
            {
                Display = "Error";
            }
        }

        [RelayCommand]
        private void GetCos()
        {
            try
            {
                double input = Convert.ToDouble(Display);
                double radians = IsRadSelected ? input : DegreesToRadians(input);
                double cos = Math.Cos(radians);
                Display = cos.ToString();
            }
            catch (FormatException)
            {
                Display = "Error";
            }
        }

        [RelayCommand]
        private void GetTan()
        {
            try
            {
                double input = Convert.ToDouble(Display);
                double radians = IsRadSelected ? input : DegreesToRadians(input);
                double tan = Math.Tan(radians);
                Display = tan.ToString();
            }
            catch (FormatException)
            {
                Display = "Error";
            }
        }

        [RelayCommand]
        private void GetSinh()
        {
            try
            {
                double sinh = Math.Sinh(Convert.ToDouble(Display));
                Display = sinh.ToString();
            }
            catch (FormatException)
            {
                Display = "Error";
            }
        }

        [RelayCommand]
        private void GetCosh()
        {
            try
            {
                double cosh = Math.Cosh(Convert.ToDouble(Display));
                Display = cosh.ToString();
            }
            catch (FormatException)
            {
                Display = "Error";
            }
        }

        [RelayCommand]
        private void GetTanh()
        {
            try
            {
                double tanh = Math.Tanh(Convert.ToDouble(Display));
                Display = tanh.ToString();
            }
            catch (FormatException)
            {
                Display = "Error";
            }
        }

        [RelayCommand]
        private void GetRand()
        {
            Random rand = new Random();
            double getRandomNumber = rand.NextDouble();
            Display = getRandomNumber.ToString();
        }

        [RelayCommand]
        private void ChangeRadSelected()
        {
            IsRadSelected = !IsRadSelected;
            NameOfRadDegButton = IsRadSelected ? "Deg" : "Rad";
        }

        [RelayCommand]
        private void ChangeFnSelected()
        {
            IsFnSelected = !IsFnSelected;
            FnBackgroundColor = IsFnSelected ? "Gray" : "#222222";
            FnTextColor = IsFnSelected ? "Black" : "White";

            IsFnUnSelected = !IsFnUnSelected;
        }

        [RelayCommand]
        private void Pow2OfX()
        {
            try
            {
                Display = Math.Pow(2, Convert.ToDouble(Display)).ToString();
            }
            catch (FormatException)
            {
                Display = "Error";
            }
        }

        [RelayCommand]
        private void Log2OfX()
        {
            try
            {
                Display = Math.Log(Convert.ToDouble(Display), 2).ToString();
            }
            catch (FormatException)
            {
                Display = "Error";
            }
        }

        [RelayCommand]
        private void GetArcSin()
        {
            try
            {
                double result = Math.Asin(Convert.ToDouble(Display));
                if (!IsRadSelected)
                {
                    result = RadiansToDegrees(result);
                }

                Display = result.ToString();
            }
            catch (FormatException)
            {
                Display = "Error";
            }
        }

        [RelayCommand]
        private void GetArcCos()
        {
            try
            {
                double result = Math.Acos(Convert.ToDouble(Display));
                if (!IsRadSelected)
                {
                    result = RadiansToDegrees(result);
                }

                Display = result.ToString();
            }
            catch (FormatException)
            {
                Display = "Error";
            }
        }

        [RelayCommand]
        private void GetArcTan()
        {
            try
            {
                double result = Math.Atan(Convert.ToDouble(Display));
                if (!IsRadSelected)
                {
                    result = RadiansToDegrees(result);
                }

                Display = result.ToString();
            }
            catch (FormatException)
            {
                Display = "Error";
            }
        }

        [RelayCommand]
        private void GetArcSinh()
        {
            try
            {
                double input = Convert.ToDouble(Display);
                Display = Math.Log(input + Math.Sqrt(input * input + 1)).ToString();
            }
            catch (FormatException)
            {
                Display = "Error";
            }
        }

        [RelayCommand]
        private void GetArcCosh()
        {
            try
            {
                double input = Convert.ToDouble(Display);
                Display = Math.Log(input + Math.Sqrt(input * input - 1)).ToString();
            }
            catch (FormatException)
            {
                Display = "Error";
            }
        }

        [RelayCommand]
        private void GetArcTanh()
        {
            try
            {
                double input = Convert.ToDouble(Display);
                Display = (0.5 * Math.Log((1 + input) / (1 - input))).ToString();
            }
            catch (FormatException)
            {
                Display = "Error";
            }
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
                if (LastOperation.EndsWith(")") ||
                    (!string.IsNullOrEmpty(LastOperation) && char.IsDigit(LastOperation.Last())))
                {
                    LastOperation += "*(";
                }
                else if (Display != "0" || _isDisplayValueUserModified)
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

        [RelayCommand]
        private void MenuClicked()
        {
            IsMenuVisible = !IsMenuVisible;
            IsButtonsVisible = !IsButtonsVisible;
        }

        [RelayCommand]
        private void MemoryClicked()
        {
            IsMemoryVisible = !IsMemoryVisible;
            IsButtonsVisible = !IsButtonsVisible;
        }

        [RelayCommand]
        private void StoreMemory()
        {
            MemoryStore.AddMemoryEntity(new MemoryEntity(long.Parse(Display)));
        }

        [RelayCommand]
        private void AddToMemory(MemoryEntity memoryEntity)
        {
            MemoryStore.AddToMemoryEntity(memoryEntity, long.Parse(Display));
        }

        [RelayCommand]
        private void SubtractFromMemory(MemoryEntity memoryEntity)
        {
            MemoryStore.SubtractFromMemoryEntity(memoryEntity, long.Parse(Display));
        }

        [RelayCommand]
        private void RemoveMemoryEntity(MemoryEntity memoryEntity)
        {
            MemoryStore.RemoveMemoryEntity(memoryEntity);
        }

        [RelayCommand]
        private void RecallMemory()
        {
            Display = MemoryStore.RecallLastMemoryEntityValue().ToString();
        }

        [RelayCommand]
        private void ClearMemory()
        {
            MemoryStore.ClearMemoryEntities();
        }
    }
}