using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace KalkulatorMAUI_MVVM.Models
{
    public class CurrencyRatesResponse
    {
        public bool Success { get; set; }
        public Dictionary<string, decimal> Rates { get; set;}
        public string Base { get; set; }
        public DateTime Date { get; set; }
    }
}
