using System.Globalization;

namespace KalkulatorMAUI_MVVM.ViewModels
{
    public class EnumBooleanConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            if (value == null || parameter == null)
                return false;

            return value.Equals(parameter);
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            if(value == null || parameter == null)
                return Binding.DoNothing;

            return (bool)value ? parameter :Binding.DoNothing;
        }
    }
}
