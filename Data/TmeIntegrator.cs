using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Newtonsoft.Json;

namespace Magazynek.Data
{
    public static class TmeIntegrator
    {
        public static async Task<ProductPricesResponse?> GetPricesAsync(string[] symbols, string token)
        {
            using var client = new HttpClient();

            var config = new ConfigurationBuilder()
                .AddJsonFile("appsettings.json", optional: true)
                .Build();

            client.DefaultRequestHeaders.Authorization = new System.Net.Http.Headers.AuthenticationHeaderValue("Bearer", token);
            var parameters = new List<KeyValuePair<string, string>>(
            [
                new KeyValuePair<string, string>("Country", "PL"),
                new KeyValuePair<string, string>("Language", "pl"),
                new KeyValuePair<string, string>("Currency", "PLN"),
                new KeyValuePair<string, string>("GrossPrices", "false"),
            ]);
            for (int i = 0; i < symbols.Length; i++)
            {
                parameters.Add(new KeyValuePair<string, string>($"SymbolList[{i}]", symbols[i]));
            }

            var content = new FormUrlEncodedContent(parameters);
            var response = await client.PostAsync("https://api.tme.eu/Products/GetPrices.json", content);
            if (!response.IsSuccessStatusCode) return null;

            var json = await response.Content.ReadAsStringAsync();
            return JsonConvert.DeserializeObject<ProductPricesResponse>(json);
        }
        public static async Task<ProductStockResponse?> GetStocksAsync(IEnumerable<string> symbols, string token)
        {
            using var client = new HttpClient();

            client.DefaultRequestHeaders.Authorization = new System.Net.Http.Headers.AuthenticationHeaderValue("Bearer", token);

            var parameters = new List<KeyValuePair<string, string>>
            {
                new("Country", "PL"),
                new("Language", "pl")
            };

            int index = 0;
            foreach (var symbol in symbols)
            {
                parameters.Add(new($"SymbolList[{index}]", symbol));
                index++;
            }

            var content = new FormUrlEncodedContent(parameters);

            var response = await client.PostAsync("https://api.tme.eu/Products/GetStocks.json", content);

            if (!response.IsSuccessStatusCode) return null;
            
            var json = await response.Content.ReadAsStringAsync();
            return JsonConvert.DeserializeObject<ProductStockResponse>(json);
        }
    }
    public class ProductPricesResponse
    {
        public string Status { get; set; } = "";
        public Data Data { get; set; } = new();
    }

    public class Data
    {
        public string Currency { get; set; } = "";
        public string Language { get; set; } = "";
        public string PriceType { get; set; } = "";
        public List<ProductItem> ProductList { get; set; } = new();
    }

    public class ProductItem
    {
        public string Symbol { get; set; } = "";
        public string Unit { get; set; } = "";
        public int VatRate { get; set; }
        public string VatType { get; set; } = "";
        public int Amount { get; set; }
        public List<Price> PriceList { get; set; } = new();
    }

    public class Price
    {
        public int Amount { get; set; }
        public decimal PriceValue { get; set; }
        public int PriceBase { get; set; }
        public bool Special { get; set; }

        public override string ToString()
        {
            return $"{Amount} {PriceValue}";
        }
    }

    public class ProductStockResponse
    {
        [JsonProperty("Status")]
        public string Status { get; set; } = "";

        [JsonProperty("Data")]
        public StockData Data { get; set; } = new();
    }

    public class StockData
    {
        [JsonProperty("ProductList")]
        public List<StockProduct> ProductList { get; set; } = new();
    }

    public class StockProduct
    {
        [JsonProperty("Symbol")]
        public string Symbol { get; set; } = "";

        [JsonProperty("Amount")]
        public int Amount { get; set; }

        [JsonProperty("Unit")]
        public string Unit { get; set; } = "";
    }
}