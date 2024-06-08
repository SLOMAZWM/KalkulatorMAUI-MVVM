using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace KalkulatorMAUI_MVVM.ViewModels
{
    public partial class CalculatorViewModel : ObservableObject
    {
        [ObservableProperty]
        private string _display = "0";

        [ObservableProperty]
        private string _operation = "";

        [ObservableProperty]
        private string _firstNumber = "0";

        [ObservableProperty]
        private string _secondNumber = "0";

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

        private bool _isOperationSet = false;

        private bool _isAfterCalculation = false;


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
        private void SetPercent()
        {
            decimal percent = Convert.ToDecimal(Display);
            decimal firstNumber = Convert.ToDecimal(FirstNumber);

            if (FirstNumber == "0" && Operation == string.Empty)
            {
                return;
            }
            else
            {
                decimal secondNumber = (firstNumber / 100) * percent;

                Display = secondNumber.ToString("");
            }
        }

        [RelayCommand]
        private void AddNumber(string number)
        {
            if (!_isAfterCalculation)
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
            else
            {
                Display = number;
                _isAfterCalculation = false;
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
            Operation = string.Empty;
            _isOperationSet = false;
            _isAfterCalculation = false;
            FirstNumber = "0";
            SecondNumber = "0";
        }

        [RelayCommand]
        private void AddDot()
        {
            if (Display.Contains("."))
            {
                return;
            }
            else
            {
                Display = Display + ".";
            }
        }

        private void Calculation()
        {
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
                        answer = Convert.ToDecimal(FirstNumber) / Convert.ToDecimal(SecondNumber);
                        break;
                    }
                case "root":
                    {
                        double firstNumber = Convert.ToDouble(FirstNumber);
                        double secondNumber = Convert.ToDouble(SecondNumber);

                        answer = Convert.ToDecimal(Math.Pow(firstNumber, 1.0 / secondNumber));
                        break;
                    }
                case "pow":
                    {
                        double firstNumber = Convert.ToDouble(FirstNumber);
                        double secondNumber = Convert.ToDouble(SecondNumber);

                        answer = Convert.ToDecimal(Math.Pow(firstNumber, secondNumber));
                        break;
                    }
                case "EE":
                    {
                        decimal firstNumber = Convert.ToDecimal(FirstNumber);
                        double secondNumber = Convert.ToDouble(SecondNumber);

                        decimal powOfTen = Convert.ToDecimal(Math.Pow(10, secondNumber));

                        answer = firstNumber * powOfTen;
                        break;
                    }
                case "powx":
                    {
                        double firstNumber = Convert.ToDouble(FirstNumber);
                        double secondNumber = Convert.ToDouble(SecondNumber);

                        answer = Convert.ToDecimal(Math.Pow(secondNumber, firstNumber));
                        break;
                    }
                case "LogBaseY":
                    {
                        double firstNumber = Convert.ToDouble(FirstNumber);
                        double secondNumber = Convert.ToDouble(SecondNumber);

                        answer = Convert.ToDecimal(Math.Log(firstNumber, secondNumber));
                        break;
                    }
            }

            Display = answer.ToString();
            _isAfterCalculation = true;
            FirstNumber = answer.ToString();
        }

        [RelayCommand]
        private void Equals()
        {
            if (_isAfterCalculation == false)
            {
                SecondNumber = Display;
                Calculation();
            }
            else
            {
                FirstNumber = Display;
                Calculation();
            }

            _isOperationSet = false;

        }

        [RelayCommand]
        private void GetSqrt()
        {
            double sqrt = Math.Sqrt(Convert.ToDouble(Display));
            Display = sqrt.ToString();
        }

        [RelayCommand]
        private void GetRoot3()
        {
            double Root3 = Math.Pow(Convert.ToDouble(Display), 1.0 / 3);
            Display = Root3.ToString();
        }

        [RelayCommand]
        private void InvertNumber()
        {
            decimal invertedNumber = 1 / Convert.ToDecimal(Display);
            Display = invertedNumber.ToString();
        }

        [RelayCommand]
        private void PowNumber()
        {
            double powNumber = Math.Pow(Convert.ToDouble(Display), 2);
            Display = powNumber.ToString();
        }

        [RelayCommand]
        private void Pow3OfNumber()
        {
            double pow3Number = Math.Pow(Convert.ToDouble(Display), 3);
            Display = pow3Number.ToString();
        }

        [RelayCommand]
        private void ExpOfX()
        {
            double expOfX = Math.Pow(Math.E, Convert.ToDouble(Display));
            Display = expOfX.ToString();
        }

        [RelayCommand]
        private void Pow10OfX()
        {
            double pow10OfX = Math.Pow(10, Convert.ToDouble(Display));
            Display = pow10OfX.ToString();
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
            double getNLog = Math.Log(Convert.ToDouble(Display), Math.E);
            Display = getNLog.ToString();
        }

        [RelayCommand]
        private void GetN10Log()
        {
            double getN10Log = Math.Log10(Convert.ToDouble(Display));
            Display = getN10Log.ToString();
        }

        [RelayCommand]
        private void GetFactorial()
        {
            double factorial = 1;
            double n = Convert.ToDouble(Display);
            for (int i = 2; i <= n; i++)
            {
                factorial *= i;
            }

            Display = factorial.ToString();
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
            double input = Convert.ToDouble(Display);
            double radians = IsRadSelected ? input : DegreesToRadians(input);
            double sin = Math.Sin(radians);

            if (!IsRadSelected)
            {
                double degrees = RadiansToDegrees(radians);
                if (degrees % 360 == 0 || degrees % 360 == 180)
                    sin = 0;
                else if (degrees % 360 == 90)
                    sin = 1;
                else if (degrees % 360 == 270)
                    sin = -1;
            }

            Display = sin.ToString();
        }

        [RelayCommand]
        private void GetCos()
        {
            double input = Convert.ToDouble(Display);
            double radians = IsRadSelected ? input : DegreesToRadians(input);
            double cos = Math.Cos(radians);

            if (!IsRadSelected)
            {
                double degrees = RadiansToDegrees(radians);
                if (degrees % 360 == 90 || degrees % 360 == 270)
                    cos = 0;
                else if (degrees % 360 == 0)
                    cos = 1;
                else if (degrees % 360 == 180)
                    cos = -1;
            }

            Display = cos.ToString();
        }

        [RelayCommand]
        private void GetTan()
        {
            double input = Convert.ToDouble(Display);
            double radians = IsRadSelected ? input : DegreesToRadians(input);
            double tan = Math.Tan(radians);

            if (!IsRadSelected)
            {
                double degrees = RadiansToDegrees(radians);
                if (degrees % 360 == 90 || degrees % 360 == 270)
                {
                    Display = "Niezdefiniowane";
                    return;
                }
            }

            Display = tan.ToString();
        }

        [RelayCommand]
        private void GetSinh()
        {
            double sinh = Convert.ToDouble(Display);
            sinh = Math.Sinh(sinh);

            Display = sinh.ToString();
        }

        [RelayCommand]
        private void GetCosh()
        {
            double cosh = Convert.ToDouble(Display);
            cosh = Math.Cosh(cosh);

            Display = cosh.ToString();
        }

        [RelayCommand]
        private void GetTanh()
        {
            double tanh = Convert.ToDouble(Display);
            tanh = Math.Tanh(tanh);

            Display = tanh.ToString();
        }

        [RelayCommand]
        private void GetRand()
        {
            Random rand = new Random();

            double GetRandomNumber = rand.NextDouble();

            Display = GetRandomNumber.ToString();
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

        //Additional Functions after Fn get Clicked

        [RelayCommand]
        private void Pow2OfX()
        {
            double pow2OfX = Convert.ToDouble(Display);
            pow2OfX = Math.Pow(2, pow2OfX);

            Display = pow2OfX.ToString();
        }

        [RelayCommand]
        private void Log2OfX()
        {
            double log2OfX = Convert.ToDouble(Display);
            log2OfX = Math.Log(log2OfX, 2);

            Display = log2OfX.ToString();
        }

        [RelayCommand]
        private void GetArcSin()
        {
            double arcSin = Convert.ToDouble(Display);
            arcSin = Math.Asin(arcSin);

            if (!IsRadSelected)
            {
                arcSin = arcSin * (180 / Math.PI);
            }

            Display = arcSin.ToString();
        }

        [RelayCommand]
        private void GetArcCos()
        {
            double arcCos = Convert.ToDouble(Display);
            arcCos = Math.Acos(arcCos);

            if (!IsRadSelected)
            {
                arcCos = arcCos * (180 / Math.PI);
            }

            Display = arcCos.ToString();
        }

        [RelayCommand]
        private void GetArcTan()
        {
            double arcTan = Convert.ToDouble(Display);
            arcTan = Math.Atan(arcTan);

            if (!IsRadSelected)
            {
                arcTan = arcTan * (180 / Math.PI);
            }

            Display = arcTan.ToString();
        }
    }
}
