using Magazynek.Data;
using Magazynek.Entities;
using Microsoft.EntityFrameworkCore;


namespace Magazynek.Services;

public interface IShippingEntryService
{
    Task<List<ShippingEntry>> Get();
    Task<List<ShippingEntryViewModel>> GetModels();
    Task<ShippingEntryViewModel> UpdateInfoOrInsertNew(ShippingEntryViewModel product, bool saveChangesAsync = true);
    Task<ShippingEntryViewModel> RefreshValue(ShippingEntryViewModel entry, bool saveChangesAsync = true);
}

public class ShippingEntryService : IShippingEntryService
{
    private readonly IDbContextFactory<DatabaseContext> dbContextFactory;
    readonly ITmeService tmeService;

    public ShippingEntryService(IDbContextFactory<DatabaseContext> dbContextFactory, ITmeService tmeService)
    {
        this.dbContextFactory = dbContextFactory;
        this.tmeService = tmeService;
    }

    public async Task<List<ShippingEntry>> Get()
    {
        await using var database = await dbContextFactory.CreateDbContextAsync();
        return await database.ShippingEntries.ToListAsync();
    }
    public async Task<List<ShippingEntryViewModel>> GetModels()
    {
        await using var database = await dbContextFactory.CreateDbContextAsync();

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
    public async Task<ShippingEntryViewModel> UpdateInfoOrInsertNew(ShippingEntryViewModel model, bool saveChangesAsync = true)
    {
        await using var database = await dbContextFactory.CreateDbContextAsync();

        ShippingEntry? entryWithSameProduct = await database.ShippingEntries.FirstOrDefaultAsync(x => x.item == model.product.id);

        if (entryWithSameProduct != null)
        {
            entryWithSameProduct.SetQuantity(entryWithSameProduct.quantity + (uint)model.quantity);
            if(saveChangesAsync) await database.SaveChangesAsync();
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
        return new ShippingEntryViewModel(dbEntry, model.product);
    }
    public async Task<ShippingEntryViewModel> RefreshValue(ShippingEntryViewModel entry, bool saveChangesAsync = true)
    {
        await using var database = await dbContextFactory.CreateDbContextAsync();
        
        Price? price = await tmeService.GetPriceForAmountAsync(entry.product.tmeID, (uint)entry.quantity);
        StockProduct? stock = await tmeService.GetStockProductAsync(entry.product.tmeID);
        if (price == null || stock == null) return entry;

        ShippingEntry? dbEntry = await database.ShippingEntries.FirstOrDefaultAsync(x => x.id == entry.id);
        if (dbEntry == null) return entry;
        dbEntry.SetLastCheck(DateTime.UtcNow);
        dbEntry.SetPrice((float)price.PriceValue);
        dbEntry.SetStock((uint)stock.Amount);
        if(saveChangesAsync) await database.SaveChangesAsync();

        return new ShippingEntryViewModel(dbEntry, entry.product);
    }
}