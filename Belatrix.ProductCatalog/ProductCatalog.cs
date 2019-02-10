using System;
using System.Collections.Generic;
using System.Fabric;
using System.Threading;
using System.Threading.Tasks;
using Belatrix.ProductCatalog.Data;
using Belatrix.ProductCatalog.Models;
using Microsoft.ServiceFabric.Services.Communication.Runtime;
using Microsoft.ServiceFabric.Services.Remoting.Runtime;
using Microsoft.ServiceFabric.Services.Runtime;

namespace Belatrix.ProductCatalog
{
    internal sealed class ProductCatalog : StatefulService, IProductCatalogService
    {
        private IProductRepository _repo;
        public ProductCatalog(StatefulServiceContext context)
            : base(context)
        { }


        public async Task AddProduct(Product product)
        {
            await _repo.AddProduct(product);
        }

        public async Task<IEnumerable<Product>> GetAllProducts()
        {
            return await _repo.GetAllProducts();
        }

        public async Task<Product> GetProduct(Guid productId)
        {
            return await _repo.GetProduct(productId);
        }

        protected override IEnumerable<ServiceReplicaListener> CreateServiceReplicaListeners()
        {
            return this.CreateServiceRemotingReplicaListeners();
        }

        protected override async Task RunAsync(CancellationToken cancellationToken)
        {
            _repo = new ProductRepository(this.StateManager);

            var product1 = new Product
            {
                Id = Guid.NewGuid(),
                Name = "Dell Monitor",
                Description = "Computer Monitor",
                Price = 500,
                Availability = 100
            };

            var product2 = new Product
            {
                Id = Guid.NewGuid(),
                Name = "Surface Book",
                Description = "Microsoft's Latest Laptop, i7 CPU, 1Tb SSD",
                Price = 2200,
                Availability = 15
            };

            var product3 = new Product
            {
                Id = Guid.NewGuid(),
                Name = "Arc Touch Mouse",
                Description = "Computer Mouse, bluetooth, requires 2 AAA batteries",
                Price = 60,
                Availability = 30
            };

            await _repo.AddProduct(product1);
            await _repo.AddProduct(product2);
            await _repo.AddProduct(product3);

            IEnumerable<Product> all = await _repo.GetAllProducts();
        }
    }
}
