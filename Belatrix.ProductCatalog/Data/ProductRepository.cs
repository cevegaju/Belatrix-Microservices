﻿using Belatrix.ProductCatalog.Models;
using Microsoft.ServiceFabric.Data;
using Microsoft.ServiceFabric.Data.Collections;
using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;

namespace Belatrix.ProductCatalog.Data
{
    class ProductRepository : IProductRepository
    {
        private IReliableStateManager _stateManager;

        public ProductRepository(IReliableStateManager stateManager)
        {
            _stateManager = stateManager;
        }

        public async Task<IEnumerable<Product>> GetAllProducts()
        {
            var products = await _stateManager.GetOrAddAsync<IReliableDictionary<Guid, Product>>("products");
            var result = new List<Product>();
            using (var tx = _stateManager.CreateTransaction())
            {
                var allProducts = await products.CreateEnumerableAsync(tx, EnumerationMode.Unordered);

                using (var enumerator = allProducts.GetAsyncEnumerator())
                {
                    while (await enumerator.MoveNextAsync(CancellationToken.None))
                    {
                        KeyValuePair<Guid, Product> current = enumerator.Current;
                        result.Add(current.Value);
                    }
                }
            }
            return result;
        }

        public async Task AddProduct(Product product)
        {
            var products = await _stateManager.GetOrAddAsync<IReliableDictionary<Guid, Product>>("products");
            using (var transaction = _stateManager.CreateTransaction())
            {
                await products.AddOrUpdateAsync(transaction, product.Id, product, (id, value) => product);
                await transaction.CommitAsync();
            }
        }

        public async Task<Product> GetProduct(Guid productId)
        {
            var products = await _stateManager.GetOrAddAsync<IReliableDictionary<Guid, Product>>("products");

            using (var transaction = _stateManager.CreateTransaction())
            {
                ConditionalValue<Product> product = await products.TryGetValueAsync(transaction, productId);

                return product.HasValue ? product.Value : null;
            }
        }
    }
}
