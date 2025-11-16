using Magazynek.Data;
using Magazynek.Entities;
using Microsoft.EntityFrameworkCore;

namespace Magazynek.Services
{
    public interface IProductService
    {
        Task<List<Product>> Get(User user);
        Task<Product?> GetByID(Guid ID);
        Task<Product> UpdateInfoOrInsertNew(ProductViewModel product, bool saveChangesAsync = true);
        Task<bool> Remove(Product product, bool saveChangesAsync = true);
    }

    public class ProductService : IProductService
    {
        readonly DatabaseContext database;

        public ProductService(DatabaseContext database) => this.database = database;
        public Task<List<Product>> Get(User user) =>  database.Products.Where(x => x.user == user.id).ToListAsync();
        public Task<Product?> GetByID(Guid id) => database.Products.FirstOrDefaultAsync(x => x.id == id);
        public async Task<Product> UpdateInfoOrInsertNew(ProductViewModel product, bool saveChangesAsync = true)
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
                dbProduct.SetType(product.type);
            }
            else
            {
                dbProduct = new Product(
                    product.name,
                    product.package,
                    product.farnellID,
                    product.tmeID,
                    product.description,
                    product.active,
                    product.type
                );
                await database.Products.AddAsync(dbProduct);
            }

            if (saveChangesAsync) await database.SaveChangesAsync();
            return dbProduct;
        }

        public async Task<bool> Remove(Product product, bool saveChangesAsync = true)
        {
            database.Products.Remove(product);
            if(saveChangesAsync) await database.SaveChangesAsync();
            return true;
        }
    }
}