using Catalog.API.Entities;
using MongoDB.Driver;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Catalog.API.Data.Contact
{
    public interface IProductContext
    {
        IMongoCollection<Products> Products { get; set; }
    }
}
