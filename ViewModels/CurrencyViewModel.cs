using System;
using System.Collections.Generic;
using System.IO;
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

            LoadCurrenciesFromFile();
        }

        private void LoadCurrenciesFromFile()
        {
            try
            {
                var filePath = "currencies.json";
                if (File.Exists(filePath))
                {
                    var json = File.ReadAllText(filePath);
                    var currencies = JsonSerializer.Deserialize<ExchangeRateApiResponse>(json);
                    if (currencies != null && currencies.ConversionRates != null)
                    {
                        AvailableCurrencies = new List<string>(currencies.ConversionRates.Keys);
                        Console.WriteLine("Available currencies loaded from file successfully.");
                    }
                    else
                    {
                        Console.WriteLine("Error parsing currencies file.");
                    }
                }
                else
                {
                    Console.WriteLine("Currencies file not found.");
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error loading currencies from file: {ex.Message}");
                Console.WriteLine(ex.ToString()); 
                DisplayCurrentExchangeRate = "An unexpected error occurred. Please try again.";
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
    }
}
