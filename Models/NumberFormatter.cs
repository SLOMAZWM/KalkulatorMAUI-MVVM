using KalkulatorMAUI_MVVM.Enums;
using KalkulatorMAUI_MVVM.ViewModels;
using System.Text;

namespace KalkulatorMAUI_MVVM.Models
{
    public static class NumberFormatter
    {
        private static string InsertSpaces(string value, int interval, bool reverse = false)
        {
            StringBuilder formatted = new StringBuilder();
            int counter = 0;

            if (reverse)
            {
                for (int i = value.Length - 1; i >= 0; i--)
                {
                    formatted.Insert(0, value[i]);
                    counter++;
                    if (counter == interval && i != 0)
                    {
                        formatted.Insert(0, ' ');
                        counter = 0;
                    }
                }
            }
            else
            {
                for (int i = 0; i < value.Length; i++)
                {
                    formatted.Append(value[i]);
                    counter++;
                    if (counter == interval && i != value.Length - 1)
                    {
                        formatted.Append(' ');
                        counter = 0;
                    }
                }
            }
            return formatted.ToString();
        }

        public static string FormatDisplay(string value, NumberSystem system)
        {
            value = value.Replace(" ", "");

            switch(system)
            {
                case NumberSystem.BIN:
                    return InsertSpaces(value, 4);
                case NumberSystem.DEC:
                    return InsertSpaces(value, 3, true);
                case NumberSystem.OCT:
                    return InsertSpaces(value, 4);
                    case NumberSystem.HEX:
                    return InsertSpaces(value, 4);
                default:
                    throw new InvalidOperationException("Nieznany system liczbowy!");
            }
        }
    }
}
