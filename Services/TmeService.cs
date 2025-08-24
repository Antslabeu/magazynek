using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Magazynek.Data;

namespace Magazynek.Services
{
    public interface ITmeService
    {
        Task<Price?> GetPriceForAmountAsync(string product, uint amount);
        Task<StockProduct?> GetStockProductAsync(string product);
    }
    public class TmeService : ITmeService
    {
        public async Task<Price?> GetPriceForAmountAsync(string product, uint amount)
        {
            ProductPricesResponse? pricesResponse = await TmeIntegrator.GetPricesAsync([product]);
            if (pricesResponse == null) return null;
            if (pricesResponse.Data.ProductList.Count == 0) return null;

            pricesResponse.Data.ProductList[0].PriceList = 
                pricesResponse.Data.ProductList[0].PriceList
                    .OrderByDescending(p => p.Amount)
                    .ToList();

            foreach (Price price in pricesResponse.Data.ProductList[0].PriceList)
            {
                if(price.Amount <= amount) return price;
            }

            return null;
        }
        public async Task<StockProduct?> GetStockProductAsync(string product)
        {
            ProductStockResponse? stockResponse = await TmeIntegrator.GetStocksAsync([product]);
            if (stockResponse == null) return null;
            if (stockResponse.Data.ProductList.Count == 0) return null;
            return stockResponse.Data.ProductList[0];
        }
    }

}