using KalkulatorMAUI_MVVM.Models;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace KalkulatorMAUI_MVVM.ViewModels
{
    public partial class CalculatorViewModel : ObservableObject
    {
        [ObservableProperty]
        private PageViewModel _pageViewModel;

        [ObservableProperty]
        private string _lastOperation = string.Empty;

        [ObservableProperty]
        private string _display = "0";

        [ObservableProperty]
        private bool _isFnSelected = false;

        [ObservableProperty]
        private bool _isFnUnSelected = true;

        [ObservableProperty]
        private string _fnBackgroundColor = "#222222";

        [ObservableProperty]
        private string _fnTextColor = "White";

        [ObservableProperty]
        private bool _isRadSelected = false;

        [ObservableProperty]
        private string _nameOfRadDegButton = "Rad";

        private bool _isAfterCalculation = false;

        [ObservableProperty]
        private bool _isMenuVisible = false;

        [ObservableProperty]
        private bool _isButtonsVisible = true;

        [ObservableProperty]
        private int _openParenthesisCount = 0;

        public CalculatorViewModel(PageViewModel pageViewModel)
        {
            PageViewModel = pageViewModel;
        }

        [RelayCommand]
        private void SetOperation(string operation)
        {
            if (Display != "0" || operation == "(" || operation == ")")
            {
                LastOperation += Display + operation;
                Display = "0";
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
        }

        [RelayCommand]
        private void AddDot()
        {
            if (!Display.Contains("."))
            {
                Display += ".";
            }
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
                DataTable table = new DataTable();
                var result = table.Compute(LastOperation, string.Empty);
                Display = Convert.ToString(result);
                LastOperation += " = " + Display;
                _isAfterCalculation = true;
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
                if (LastOperation.EndsWith(")") || (!string.IsNullOrEmpty(LastOperation) && char.IsDigit(LastOperation.Last())))
                {
                    LastOperation += "*(";
                }
                else if (Display != "0")
                {
                    LastOperation += Display + "*(";
                    Display = "0";
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
                if (Display == "0")
                {
                    LastOperation += ")";
                }
                else
                {
                    LastOperation += Display + ")";
                    Display = "0";
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
    }
}
