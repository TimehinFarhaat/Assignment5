using System.Text.Json;

namespace CSharpMvcBasics.Implementation.Service
{
    public class CurrencyService:ICurrencyService
    {
        private readonly HttpClient _httpClient;

        public CurrencyService(HttpClient httpClient)
        {
            _httpClient = httpClient;
        }

        public async Task<decimal> ConvertCurrencyAsync(string fromCurrency, string toCurrency, decimal amount)
        {
            var url = $"https://open.er-api.com/v6/latest/{fromCurrency}";

            var response = await _httpClient.GetAsync(url);

            if (!response.IsSuccessStatusCode)
                throw new Exception("Failed to fetch exchange rates");

            var json = JsonDocument.Parse(await response.Content.ReadAsStringAsync());

            var root = json.RootElement;

            if (!root.TryGetProperty("rates", out var rates) ||
                !rates.TryGetProperty(toCurrency, out var targetRate))
                throw new Exception($"Currency '{toCurrency}' not found in exchange rates.");

            var rate = targetRate.GetDecimal();

            return amount * rate;
        }
    }
}
