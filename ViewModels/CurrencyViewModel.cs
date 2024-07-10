using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Net.Http.Json;
using System.Threading.Tasks;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using KalkulatorMAUI_MVVM.Models;

namespace KalkulatorMAUI_MVVM.ViewModels
{
    public partial class CurrencyViewModel : ObservableObject
    {
        private readonly HttpClient _httpClient;
        private const string ApiUrl = "https://api.exchangeratesapi.io/latest?base=USD&access_key=bbe7a4b635a24a4f0667dcfd83c12016";

        [ObservableProperty]
        private string _selectedCurrencyFrom = "";

        [ObservableProperty]
        private string _selectedCurrencyTo = "";

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
            _httpClient.DefaultRequestHeaders.Add("apikey", "bbe7a4b635a24a4f0667dcfd83c12016");
            Task.Run(async () => await LoadCurrenciesAsync());
        }

        [RelayCommand]
        private async Task LoadCurrenciesAsync()
        {
            try
            {
                HttpResponseMessage response = await _httpClient.GetAsync(ApiUrl);
                response.EnsureSuccessStatusCode();

                CurrencyRatesResponse currencyResponse = await response.Content.ReadFromJsonAsync<CurrencyRatesResponse>();
                if (currencyResponse != null)
                {
                    AvailableCurrencies = currencyResponse.Rates.Keys.ToList();
                    DisplayLastUpdate = currencyResponse.Date.ToString("yyyy-MM-dd HH:mm:ss");
                    Console.WriteLine("Available Currencies Loaded: " + string.Join(", ", AvailableCurrencies)); // Debug
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine("Błąd: " + ex.Message);
            }
        }

        [RelayCommand]
        private async Task UpdateExchangeRateAsync()
        {
            if (string.IsNullOrEmpty(SelectedCurrencyFrom) || string.IsNullOrEmpty(SelectedCurrencyTo)) return;

            decimal rate = await GetExchangeRateAsync(SelectedCurrencyFrom, SelectedCurrencyTo);
            DisplayCurrencyFrom = SelectedCurrencyFrom;
            DisplayCurrencyTo = SelectedCurrencyTo;
            DisplayCurrentExchangeRate = $"{rate} {SelectedCurrencyTo}";
        }

        private async Task<decimal> GetExchangeRateAsync(string fromCurrency, string toCurrency)
        {
            try
            {
                HttpResponseMessage response = await _httpClient.GetAsync(ApiUrl);
                response.EnsureSuccessStatusCode();

                CurrencyRatesResponse currencyResponse = await response.Content.ReadFromJsonAsync<CurrencyRatesResponse>();
                if (currencyResponse != null && currencyResponse.Rates.TryGetValue(toCurrency, out decimal rate))
                {
                    return rate;
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine("Błąd: " + ex.Message);
            }
            return 1.0M; 
        }

    }
}
