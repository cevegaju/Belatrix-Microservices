﻿using Microsoft.ServiceFabric.Services.Remoting;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Belatrix.ProductCatalog.Models
{
    public interface IProductCatalogService : IService
    {
        Task<IEnumerable<Product>> GetAllProducts();

        Task AddProduct(Product product);
        Task<Product> GetProduct(Guid productId);
    }
}
