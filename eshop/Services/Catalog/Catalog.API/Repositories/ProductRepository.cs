using Catalog.API.Data.Contact;
using Catalog.API.Entities;
using Catalog.API.Repositories.Contract;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using MongoDB.Driver;

namespace Catalog.API.Repositories
{
    public class ProductRepository : IProductRepository
    {

        private readonly IProductContext productContext;

        public ProductRepository(IProductContext productContext)
        {
            this.productContext = productContext;
        }

        public async Task CreateProduct(Products product)
        {
            await productContext.Product.InsertOneAsync(product);
        }

        public async Task<bool> DeleteProduct(string id)
        {
            var result = await productContext.Product.DeleteOneAsync(x => x.Id == id);

            return result.IsAcknowledged && result.DeletedCount>0;
        }

        public async Task<Products> GetProduct(string id)
        {
            return await productContext
                .Product.Find(filter: x => x.Id == id).FirstOrDefaultAsync();
        }

        public async Task<IEnumerable<Products>> GetProductByCategory(string categoryName)
        {
            return await productContext.
                Product.Find(filter: x => x.Category == categoryName).ToListAsync();
        }

        public async Task<IEnumerable<Products>> GetProductByName(string name)
        {
            return await productContext.
    Product.Find(filter: x => x.Name == name).ToListAsync();
        }

        public async Task<IEnumerable<Products>> GetProducts()
        {
            return await productContext.Product.Find(x => true).ToListAsync();
        }

        public async Task<bool> UpdateProduct(Products product)
        {
            var result = await productContext.Product
                .ReplaceOneAsync(filter: x => x.Id == product.Id, replacement: product);
            return result.IsAcknowledged && result.ModifiedCount>0;
        }
    }
}
