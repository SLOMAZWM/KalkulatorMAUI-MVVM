using KalkulatorMAUI_MVVM.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http.Json;
using System.Text;
using System.Threading.Tasks;

namespace KalkulatorMAUI_MVVM.ViewModels
{
    public partial class CurrencyViewModel : ObservableObject
    {
        private readonly HttpClient _httpClient;
        private const string ApiUrl = "https://api.apilayer.com/exchangerates_data/latest?base=USD";

        [ObservableProperty]
        private string _selectedCurrencyFrom;

        [ObservableProperty]
        private string _selectedCurrencyTo;

        [ObservableProperty]
        private string _displayCurrencyFrom = "";

        [ObservableProperty]
        private string _displayCurrencyTo = "";

        [ObservableProperty]
        private string _displayCurrentExchangeRate = "";

        [ObservableProperty]
        private string _displayLastUpdate = "";

        [ObservableProperty]
        private List<string> _availableCurrencies;

        public CurrencyViewModel()
        {
            AvailableCurrencies = new List<string>();
            _httpClient = new HttpClient();
            _httpClient.DefaultRequestHeaders.Add("apikey", "846bdfa23944086ec19dc33dfde9287c");
        }

        [RelayCommand]
        private async Task LoadCurrenciesAsync()
        {
            try
            {
                var response = await _httpClient.GetFromJsonAsync<CurrencyRatesResponse>(ApiUrl);
                if(response != null && response.Success)
                {
                    AvailableCurrencies = response.Rates.Keys.ToList();
                    DisplayLastUpdate = response.Date.ToString("yyyy-MM-dd HH:mm:ss");
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine("Błąd: " + ex.Message);
            }
        }

        private void UpdateExchangeRate()
        {
            if (string.IsNullOrEmpty(SelectedCurrencyFrom) || string.IsNullOrEmpty(SelectedCurrencyTo)) return;

            var rate = GetExchangeRate(SelectedCurrencyFrom, SelectedCurrencyTo);
            DisplayCurrencyFrom = SelectedCurrencyFrom;
            DisplayCurrencyTo = SelectedCurrencyTo;
            DisplayCurrentExchangeRate = $"{rate} {SelectedCurrencyTo}";
        }

        private decimal GetExchangeRate(string fromCurrency, string toCurrency)
        {
            var response = _httpClient.GetFromJsonAsync<CurrencyRatesResponse>(ApiUrl).Result;

            if(response.Rates.TryGetValue(toCurrency, out var rate))
            {
                return rate;
            }
            return 1.0M;
        }
    }
}
