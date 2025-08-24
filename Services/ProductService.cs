using Magazynek.Data;
using Magazynek.Entities;
using Microsoft.EntityFrameworkCore;

namespace Magazynek.Services
{
    public interface IProductService
    {
        Task<List<Product>> Get();
        Task<Product?> GetByID(Guid ID);
        Task<Product> UpdateInfoOrInsertNew(ProductViewModel product);
        Task<bool> Remove(Product product);
    }

    public class ProductService : IProductService
    {
        readonly DatabaseContext database;

        public ProductService(DatabaseContext database) => this.database = database;
        public Task<List<Product>> Get() => database.Products.ToListAsync();
        public Task<Product?> GetByID(Guid id) => database.Products.FirstOrDefaultAsync(x => x.id == id);
        public async Task<Product> UpdateInfoOrInsertNew(ProductViewModel product)
        {
            Product? dbProduct = await database.Products.FirstOrDefaultAsync(x => x.id == product.id);
            if (dbProduct != null)
            {
                dbProduct.SetName(product.name);
                dbProduct.SetPackage(product.package);
                dbProduct.SetFarnellID(product.farnellID);
                dbProduct.SetTmeID(product.tmeID);
                dbProduct.SetDescription(product.description);
                dbProduct.SetActive(product.active);
            }
            else
            {
                dbProduct = new Product(
                    product.name,
                    product.package,
                    product.farnellID,
                    product.tmeID,
                    product.description,
                    product.active
                );
                await database.Products.AddAsync(dbProduct);
            }

            await database.SaveChangesAsync();
            return dbProduct;
        }

        public async Task<bool> Remove(Product product)
        {
            database.Products.Remove(product);
            await database.SaveChangesAsync();
            return true;
        }
    }
}