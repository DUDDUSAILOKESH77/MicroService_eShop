using Catalog.API.Data.Contact;
using Catalog.API.Entities;
using Microsoft.Extensions.Configuration;
using MongoDB.Driver;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Catalog.API.Data
{
    public class ProductContext : IProductContext
    {
        public ProductContext(IConfiguration configuration)
        {
            var client = new MongoClient(configuration.GetValue<string>("DatabaseSettings:ConnectionString"));
            var connectionString = client.
                GetDatabase(configuration.GetValue<string>("DatabaseSettings:DatabaseName"));
            var Product = connectionString.GetCollection<Products>(configuration.GetValue<string>("DatabaseSettings:CollectionName"));
            ProductsContextSeed.SeedData(Product);
        }

        public IMongoCollection<Products> Product { get; set; }
    }
}
