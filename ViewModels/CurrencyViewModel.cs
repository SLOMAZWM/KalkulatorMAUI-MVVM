using System;
using System.Collections.Generic;
using System.Net.Http;
using System.Threading.Tasks;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using Newtonsoft.Json.Linq;
using System.Linq;

namespace KalkulatorMAUI_MVVM.ViewModels
{
    public partial class CurrencyViewModel : ObservableObject
    {
        private readonly HttpClient _httpClient;

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

        [ObservableProperty]
        private bool _isLoading;

        private bool _isFirstSign = true;
        private bool _isDotSet = false;

        public CurrencyViewModel(PageViewModel pageViewModel)
        {
            PageViewModel = pageViewModel;
            _httpClient = new HttpClient();
            LoadAvailableCurrenciesCommand = new AsyncRelayCommand(LoadAvailableCurrenciesAsync);
            UpdateExchangeAsyncCommand = new AsyncRelayCommand(UpdateExchangeAsync);

            LoadAvailableCurrenciesCommand.Execute(null);
        }

        public IAsyncRelayCommand LoadAvailableCurrenciesCommand { get; }
        public IAsyncRelayCommand UpdateExchangeAsyncCommand { get; }

        private async Task LoadAvailableCurrenciesAsync()
        {
            string url = "https://open.er-api.com/v6/latest/USD";
            try
            {
                var response = await _httpClient.GetStringAsync(url);
                var data = JObject.Parse(response);

                if (data["rates"] is JObject rates)
                {
                    AvailableCurrencies = new List<string>(rates.Properties().Select(p => p.Name));
                }
                else
                {
                    Console.WriteLine("Rates not found in API response.");
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error fetching currencies: {ex.Message}");
            }
        }

        private async Task<double> GetExchangeRateFromApiAsync(string fromCurrency, string toCurrency)
        {
            string url = $"https://open.er-api.com/v6/latest/{fromCurrency}";
            try
            {
                var response = await GetStringWithRetriesAsync(url);

                if (response == null)
                {
                    Console.WriteLine("Failed to get response after retries.");
                    return 0;
                }

                var data = JObject.Parse(response);
                if (data["rates"]?[toCurrency] is JToken rateToken && double.TryParse(rateToken.ToString(), out double rate))
                {
                    return rate;
                }
                else
                {
                    Console.WriteLine("Rate not found or unable to parse rate.");
                }
            }
            catch (HttpRequestException httpEx)
            {
                Console.WriteLine($"HTTP Request Exception: {httpEx.Message}");
            }
            catch (Exception ex)
            {
                Console.WriteLine($"General Exception: {ex.Message}");
            }

            return 0;
        }

        private async Task<string> GetStringWithRetriesAsync(string url, int maxRetries = 3)
        {
            int retryCount = 0;
            while (retryCount < maxRetries)
            {
                try
                {
                    return await _httpClient.GetStringAsync(url);
                }
                catch (HttpRequestException httpEx)
                {
                    Console.WriteLine($"HTTP Request Exception on attempt {retryCount + 1}: {httpEx.Message}");
                    if (retryCount == maxRetries - 1) throw;
                }
                retryCount++;
                await Task.Delay(2000);
            }
            return null;
        }

        [RelayCommand]
        private async Task UpdateExchangeAsync()
        {
            IsLoading = true;
            try
            {
                if (string.IsNullOrEmpty(SelectedCurrencyFrom) || string.IsNullOrEmpty(SelectedCurrencyTo) || string.IsNullOrEmpty(DisplayCurrencyFrom))
                {
                    DisplayCurrentExchangeRate = "Please select both currencies and enter an amount.";
                    return;
                }

                if (!double.TryParse(DisplayCurrencyFrom, out double amount))
                {
                    DisplayCurrentExchangeRate = "Please enter a valid amount.";
                    return;
                }

                Console.WriteLine($"Attempting to fetch exchange rate for {SelectedCurrencyFrom} to {SelectedCurrencyTo}...");
                var rate = await GetExchangeRateFromApiAsync(SelectedCurrencyFrom, SelectedCurrencyTo);
                if (rate == 0)
                {
                    DisplayCurrentExchangeRate = "Error updating exchange rate. Please check your internet connection.";
                    return;
                }

                var convertedAmount = amount * rate;

                DisplayCurrentExchangeRate = $"1 {SelectedCurrencyFrom} = {rate} {SelectedCurrencyTo}";
                DisplayCurrencyTo = convertedAmount.ToString();
                DisplayLastUpdate = $"Last update: {DateTime.UtcNow}";
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
            finally
            {
                IsLoading = false;
            }
        }

        [RelayCommand]
        private void EnterSign(string sign)
        {
            if (_isFirstSign)
            {
                DisplayCurrencyFrom = sign;
                _isFirstSign = false;
            }
            else
            {
                DisplayCurrencyFrom += sign;
            }
        }

        [RelayCommand]
        private void ClearDisplay()
        {
            DisplayCurrencyFrom = "";
            DisplayCurrencyTo = "";
            _isFirstSign = true;
            _isDotSet = false;
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
            if (!_isDotSet)
            {
                DisplayCurrencyFrom += ".";
                _isDotSet = true;
            }
        }
    }
}
