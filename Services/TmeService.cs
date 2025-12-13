using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Magazynek.Data;
using Magazynek.Entities;

namespace Magazynek.Services
{
    public interface ITmeService
    {
        Task<Price?> GetPriceForAmountAsync(string product, uint amount, User user);
        Task<StockProduct?> GetStockProductAsync(string product, User user);
    }
    public class TmeService : ITmeService
    {
        private readonly ISystemSettingsService settingsService;

        public TmeService(ISystemSettingsService settingsService)
        {
            this.settingsService = settingsService;
        }
        public async Task<Price?> GetPriceForAmountAsync(string product, uint amount, User user)
        {
            string token = await settingsService.GetSetting<string>("TME API token", user);
            
            ProductPricesResponse? pricesResponse = await TmeIntegrator.GetPricesAsync([product], token);
            if (pricesResponse == null) return null;
            if (pricesResponse.Data.ProductList.Count == 0) return null;
            if (pricesResponse.Data.ProductList[0].PriceList.Count == 0) return null;

            if (amount < pricesResponse.Data.ProductList[0].PriceList[0].Amount) amount = (uint)pricesResponse.Data.ProductList[0].PriceList[0].Amount;

            pricesResponse.Data.ProductList[0].PriceList =
                pricesResponse.Data.ProductList[0].PriceList
                    .OrderByDescending(p => p.Amount)
                    .ToList();

            foreach (Price price in pricesResponse.Data.ProductList[0].PriceList)
            {
                if (price.Amount <= amount) return price;
            }

            return null;
        }
        public async Task<StockProduct?> GetStockProductAsync(string product, User user)
        {
            string token = await settingsService.GetSetting<string>("TME API token", user);

            ProductStockResponse? stockResponse = await TmeIntegrator.GetStocksAsync([product], token);
            if (stockResponse == null) return null;
            if (stockResponse.Data.ProductList.Count == 0) return null;
            return stockResponse.Data.ProductList[0];
        }
    }

}