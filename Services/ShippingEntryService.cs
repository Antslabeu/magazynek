using Magazynek.Data;
using Magazynek.Entities;
using Microsoft.EntityFrameworkCore;


namespace Magazynek.Services;

public interface IShippingEntryService
{
    Task<List<ShippingEntry>> Get();
    Task<List<ShippingEntryViewModel>> GetModels();
    Task<ShippingEntryViewModel> UpdateInfoOrInsertNew(ShippingEntryViewModel product);
    Task<ShippingEntryViewModel> RefreshValue(ShippingEntryViewModel entry);
}

public class ShippingEntryService : IShippingEntryService
{
    readonly DatabaseContext database;
    readonly ITmeService tmeService;

    public ShippingEntryService(DatabaseContext database, ITmeService tmeService)
    {
        this.database = database;
        this.tmeService = tmeService;
    }

    public Task<List<ShippingEntry>> Get() => database.ShippingEntries.ToListAsync();
    public async Task<List<ShippingEntryViewModel>> GetModels()
    {
        List<ShippingEntryViewModel> models = new();
        List<ShippingEntry> entries = await database.ShippingEntries.ToListAsync();
        List<Product> products = await database.Products.ToListAsync();
        foreach (ShippingEntry entry in entries)
        {
            Product? product = products.FirstOrDefault(p => p.id == entry.item);
            if (product == null) continue;
            models.Add(new ShippingEntryViewModel(entry, product));
        }
        return models;
    }
    public async Task<ShippingEntryViewModel> UpdateInfoOrInsertNew(ShippingEntryViewModel model)
    {
        ShippingEntry? entryWithSameProduct = await database.ShippingEntries.FirstOrDefaultAsync(x => x.item == model.product.id);

        if (entryWithSameProduct != null)
        {
            entryWithSameProduct.SetQuantity(entryWithSameProduct.quantity + (uint)model.quantity);
            await database.SaveChangesAsync();
            return new ShippingEntryViewModel(entryWithSameProduct, model.product);
        }


        float priceValue = 0;
        uint stockValue = 0;
        Price? price = await tmeService.GetPriceForAmountAsync(model.product.tmeID, (uint)model.quantity);
        StockProduct? stock = await tmeService.GetStockProductAsync(model.product.tmeID);
        if (price != null && stock != null)
        {
            priceValue = (float)price.PriceValue;
            stockValue = (uint)stock.Amount;
        }


        ShippingEntry? dbEntry = await database.ShippingEntries.FirstOrDefaultAsync(x => x.id == model.id);
        if (dbEntry != null)
        {
            dbEntry.SetItem(model.product.id);
            dbEntry.SetQuantity((uint)model.quantity);
            dbEntry.SetLastCheck(DateTime.UtcNow);
            dbEntry.SetStock(stockValue);
            dbEntry.SetPrice(priceValue);
        }
        else
        {
            dbEntry = new ShippingEntry(model.product.id, (uint)model.quantity, DateTime.UtcNow, stockValue, priceValue);
            await database.ShippingEntries.AddAsync(dbEntry);
        }
        await database.SaveChangesAsync();
        return new ShippingEntryViewModel(dbEntry, model.product); ;
    }
    public async Task<ShippingEntryViewModel> RefreshValue(ShippingEntryViewModel entry)
    {
        Price? price = await tmeService.GetPriceForAmountAsync(entry.product.tmeID, (uint)entry.quantity);
        StockProduct? stock = await tmeService.GetStockProductAsync(entry.product.tmeID);
        if (price == null || stock == null) return entry;

        ShippingEntry? dbEntry = await database.ShippingEntries.FirstOrDefaultAsync(x => x.id == entry.id);
        if (dbEntry == null) return entry;
        dbEntry.SetLastCheck(DateTime.UtcNow);
        dbEntry.SetPrice((float)price.PriceValue);
        dbEntry.SetStock((uint)stock.Amount);
        await database.SaveChangesAsync();

        return new ShippingEntryViewModel(dbEntry, entry.product);
    }
}