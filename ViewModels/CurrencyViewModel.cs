using System;
using System.Collections.Generic;
using System.Net.Http;
using System.Net.Http.Json;
using System.Threading.Tasks;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using Microsoft.Extensions.Configuration;
using KalkulatorMAUI_MVVM.Models;
using System.Text.Json;

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

            try
            {
                var builder = new ConfigurationBuilder()
                    .AddUserSecrets<CurrencyViewModel>()
                    .AddJsonFile("Properties/launchSettings.json", optional: false, reloadOnChange: true);

                var configuration = builder.Build();
                _apiKey = configuration["ApiKey"];
                Console.WriteLine($"API Key Loaded: {_apiKey}");

                _httpClient.DefaultRequestHeaders.Add("Accept", "application/json");

                LoadCurrenciesFromConfig(configuration);
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error loading API key or currencies: {ex.Message}");
                DisplayCurrentExchangeRate = "Error loading configuration. Please check your settings.";
                return;
            }
        }

        private void LoadCurrenciesFromConfig(IConfiguration configuration)
        {
            try
            {
                var currencyCodes = configuration.GetSection("Currencies:codes").Get<List<string>>();
                if (currencyCodes != null)
                {
                    AvailableCurrencies = currencyCodes;
                    Console.WriteLine("Available currencies loaded from configuration successfully.");
                    Console.WriteLine($"Currencies: {string.Join(", ", AvailableCurrencies)}");
                }
                else
                {
                    Console.WriteLine("Error loading currencies from configuration.");
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error loading currencies from configuration: {ex.Message}");
                Console.WriteLine(ex.ToString());
                DisplayCurrentExchangeRate = "An unexpected error occurred. Please try again.";
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
    }
}
