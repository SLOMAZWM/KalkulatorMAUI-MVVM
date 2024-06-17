using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace KalkulatorMAUI_MVVM.ViewModels
{
    public partial class ProgrammerViewModel : ObservableObject
    {
        [ObservableProperty]
        private string _display = "0";

        private bool _firstSign = true;

        [RelayCommand]
        private void EnterToDisplay(string Sign)
        {
            if(_firstSign)
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
    }
}
