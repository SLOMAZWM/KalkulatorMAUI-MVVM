using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Net.Http.Json;
using System.Text.Json;
using System.Threading.Tasks;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using KalkulatorMAUI_MVVM.Models;

namespace KalkulatorMAUI_MVVM.ViewModels
{
    public partial class CurrencyViewModel : ObservableObject
    {
        private readonly HttpClient _httpClient;
        private readonly string _apiKey;

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

        [ObservableProperty]
        private PageViewModel _pageViewModel;

        public CurrencyViewModel(PageViewModel pageViewModel)
        {
            AvailableCurrencies = new List<string>();
            PageViewModel = pageViewModel;
            _httpClient = new HttpClient();

            var config = JsonSerializer.Deserialize<Dictionary<string, string>>(File.ReadAllText("config.json"));
            _apiKey = config["ApiKey"];
            InitializeCurrencies();
        }

        private async Task InitializeCurrencies()
        {
            try
            {
                var response = await _httpClient.GetFromJsonAsync<ExchangeRateApiResponse>("https://api.exchangeratesapi.io/latest?apikey=YOUR_API_KEY");
                AvailableCurrencies = response.Rates.Keys.ToList();
            }
            catch
            {
                Console.WriteLine("Błąd przy pobieraniu walut!");
            }
        }

        [RelayCommand]
        private async Task UpdateExchangeAsync()
        {
            try
            {
                if (string.IsNullOrEmpty(SelectedCurrencyFrom) || string.IsNullOrEmpty(SelectedCurrencyTo))
                {
                    DisplayCurrentExchangeRate = "Please select both currencies.";
                    return;
                }

                var response = await _httpClient.GetFromJsonAsync<ExchangeRateApiResponse>($"https://api.exchangeratesapi.io/latest?base={SelectedCurrencyFrom}&symbols={SelectedCurrencyTo}&apikey=YOUR_API_KEY");
                var rate = response.Rates[SelectedCurrencyTo];

                DisplayCurrentExchangeRate = $"1 {SelectedCurrencyFrom} = {rate} {SelectedCurrencyTo}";
                DisplayLastUpdate = $"Last update: {response.Date}";
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error updating exchange rate: {ex.Message}");
            }
        }

    }
}
