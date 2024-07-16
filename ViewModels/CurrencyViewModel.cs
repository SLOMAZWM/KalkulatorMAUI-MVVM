using System;
using System.Collections.Generic;
using System.Net.Http;
using System.Net.Http.Json;
using System.Threading.Tasks;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using KalkulatorMAUI_MVVM.Models;
using System.Text.Json;
using Microsoft.Extensions.Configuration;

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

        private bool _isFirstSign = true;

        private bool _isDotSet = false;

        public CurrencyViewModel(PageViewModel pageViewModel)
        {
            AvailableCurrencies = new List<string>
            {
                "USD", "AED", "AFN", "ALL", "AMD", "ANG", "AOA", "ARS", "AUD", "AWG",
                "AZN", "BAM", "BBD", "BDT", "BGN", "BHD", "BIF", "BMD", "BND", "BOB",
                "BRL", "BSD", "BTN", "BWP", "BYN", "BZD", "CAD", "CDF", "CHF", "CLP",
                "CNY", "COP", "CRC", "CUP", "CVE", "CZK", "DJF", "DKK", "DOP", "DZD",
                "EGP", "ERN", "ETB", "EUR", "FJD", "FKP", "FOK", "GBP", "GEL", "GGP",
                "GHS", "GIP", "GMD", "GNF", "GTQ", "GYD", "HKD", "HNL", "HRK", "HTG",
                "HUF", "IDR", "ILS", "IMP", "INR", "IQD", "IRR", "ISK", "JEP", "JMD",
                "JOD", "JPY", "KES", "KGS", "KHR", "KID", "KMF", "KRW", "KWD", "KYD",
                "KZT", "LAK", "LBP", "LKR", "LRD", "LSL", "LYD", "MAD", "MDL", "MGA",
                "MKD", "MMK", "MNT", "MOP", "MRU", "MUR", "MVR", "MWK", "MXN", "MYR",
                "MZN", "NAD", "NGN", "NIO", "NOK", "NPR", "NZD", "OMR", "PAB", "PEN",
                "PGK", "PHP", "PKR", "PLN", "PYG", "QAR", "RON", "RSD", "RUB", "RWF",
                "SAR", "SBD", "SCR", "SDG", "SEK", "SGD", "SHP", "SLE", "SLL", "SOS",
                "SRD", "SSP", "STN", "SYP", "SZL", "THB", "TJS", "TMT", "TND", "TOP",
                "TRY", "TTD", "TVD", "TWD", "TZS", "UAH", "UGX", "UYU", "UZS", "VES",
                "VND", "VUV", "WST", "XAF", "XCD", "XDR", "XOF", "XPF", "YER", "ZAR",
                "ZMW", "ZWL"
            };
            PageViewModel = pageViewModel;
            _httpClient = new HttpClient();

            try
            {
                var builder = new ConfigurationBuilder()
                    .AddUserSecrets<CurrencyViewModel>();

                var configuration = builder.Build();
                _apiKey = configuration["ApiKey"];
                Console.WriteLine($"API Key Loaded: {_apiKey}");

                _httpClient.DefaultRequestHeaders.Add("Accept", "application/json");
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error loading API key: {ex.Message}");
                DisplayCurrentExchangeRate = "Error loading API key. Please check your configuration.";
                return;
            }
        }

        [RelayCommand]
        public async Task UpdateExchangeAsync()
        {
            try
            {
                if (string.IsNullOrEmpty(SelectedCurrencyFrom) || string.IsNullOrEmpty(SelectedCurrencyTo))
                {
                    DisplayCurrentExchangeRate = "Please select both currencies.";
                    return;
                }

                Console.WriteLine($"Attempting to fetch exchange rate for {SelectedCurrencyFrom} to {SelectedCurrencyTo}...");
                var response = await _httpClient.GetFromJsonAsync<ExchangeRatePairResponse>($"https://v6.exchangerate-api.com/v6/{_apiKey}/pair/{SelectedCurrencyFrom}/{SelectedCurrencyTo}");
                if (response == null)
                {
                    Console.WriteLine("Response is null");
                    DisplayCurrentExchangeRate = "Error updating exchange rate. Please check your internet connection.";
                    return;
                }
                if (response.Result != "success")
                {
                    Console.WriteLine($"API response error: {response.Result}");
                    DisplayCurrentExchangeRate = $"Error updating exchange rate: {response.Result}";
                    return;
                }
                var rate = response.ConversionRate;

                DisplayCurrentExchangeRate = $"1 {SelectedCurrencyFrom} = {rate} {SelectedCurrencyTo}";
                DisplayLastUpdate = $"Last update: {response.TimeLastUpdateUtc}";
                Console.WriteLine($"Exchange rate fetched successfully: 1 {SelectedCurrencyFrom} = {rate} {SelectedCurrencyTo}");
            }
            catch (HttpRequestException httpEx)
            {
                Console.WriteLine($"HTTP Error: {httpEx.Message}");
                Console.WriteLine(httpEx.ToString());
                DisplayCurrentExchangeRate = "Error updating exchange rate. Please check your internet connection.";
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error: {ex.Message}");
                Console.WriteLine(ex.ToString());
                DisplayCurrentExchangeRate = "An unexpected error occurred. Please try again.";
            }
        }

        public IRelayCommand UpdateExchangeAsyncCommand => new RelayCommand(async () => await UpdateExchangeAsync());

        [RelayCommand]
        private void EnterSign(string Sign)
        {
            if (_isFirstSign)
            {
                DisplayCurrencyFrom = Sign;
                _isFirstSign = false;
            }
            else
            {
                DisplayCurrencyFrom += Sign;
            }
        }

        [RelayCommand]
        private void ClearDisplay()
        {
            DisplayCurrencyFrom = "";
            DisplayCurrencyTo = "";
            _isFirstSign = true;
        }

        [RelayCommand]
        private void DeleteSign()
        {
            if (DisplayCurrencyFrom.Length > 0)
            {
                DisplayCurrencyFrom = DisplayCurrencyFrom.Remove(DisplayCurrencyFrom.Length - 1);
            }
        }

        [RelayCommand]
        private void DotSet()
        {
            if(!_isDotSet)
            {
                DisplayCurrencyFrom += ".";
                _isDotSet = true;
            }
            else
            {
                return;
            }
        }
    }
}

