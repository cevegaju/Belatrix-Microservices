using System;
using System.Collections.Generic;
using System.Fabric;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Belatrix.CheckoutService.Models;
using Belatrix.ProductCatalog.Models;
using Belatrix.UserActor.Interfaces;
using Microsoft.ServiceFabric.Actors;
using Microsoft.ServiceFabric.Actors.Client;
using Microsoft.ServiceFabric.Data.Collections;
using Microsoft.ServiceFabric.Services.Client;
using Microsoft.ServiceFabric.Services.Communication.Runtime;
using Microsoft.ServiceFabric.Services.Remoting.Client;
using Microsoft.ServiceFabric.Services.Remoting.Runtime;
using Microsoft.ServiceFabric.Services.Runtime;


namespace Belatrix.CheckoutService
{
    internal sealed class CheckoutService : StatefulService, ICheckoutService
    {
        public CheckoutService(StatefulServiceContext context)
            : base(context)
        { }

        public async Task<CheckoutSummary> Checkout(string userId)
        {
            var result = new CheckoutSummary();
            result.Date = DateTime.UtcNow;
            result.Products = new List<CheckoutProduct>();
                        
            var userActor = GetUserActor(userId);
            Dictionary<Guid, int> basket = await userActor.GetBasket();
                        
            var catalogService = GetProductCatalogService();
                        
            foreach (KeyValuePair<Guid, int> basketLine in basket)
            {
                var product = await catalogService.GetProduct(basketLine.Key);
                var checkoutProduct = new CheckoutProduct
                {
                    Product = product,
                    Price = product.Price,
                    Quantity = basketLine.Value
                };
                result.Products.Add(checkoutProduct);
            }
                        
            result.TotalPrice = result.Products.Sum(p => p.Price);
                        
            await userActor.ClearBasket();

            await AddToHistory(result);

            return result;
        }

        public async Task<IEnumerable<CheckoutSummary>> GetOrderHistory(string userId)
        {
            var result = new List<CheckoutSummary>();
            var history = await StateManager.GetOrAddAsync<IReliableDictionary<DateTime, CheckoutSummary>>("history");

            using (var tx = StateManager.CreateTransaction())
            {
                var allProducts = await history.CreateEnumerableAsync(tx, EnumerationMode.Unordered);
                using (var enumerator = allProducts.GetAsyncEnumerator())
                {
                    while (await enumerator.MoveNextAsync(CancellationToken.None))
                    {
                        KeyValuePair<DateTime, CheckoutSummary> current = enumerator.Current;

                        result.Add(current.Value);
                    }
                }
            }

            return result;
        }

        private async Task AddToHistory(CheckoutSummary checkout)
        {
            var history = await StateManager.GetOrAddAsync<IReliableDictionary<DateTime, CheckoutSummary>>("history");

            using (var transaction = StateManager.CreateTransaction())
            {
                await history.AddAsync(transaction, checkout.Date, checkout);

                await transaction.CommitAsync();
            }
        }

        private IUserActor GetUserActor(string userId)
        {
            return ActorProxy.Create<IUserActor>(new ActorId(userId), new Uri("fabric:/Belatrix/UserActorService"));
        }

        private IProductCatalogService GetProductCatalogService()
        {
            return ServiceProxy.Create<IProductCatalogService>(
               new Uri("fabric:/Belatrix/Belatrix.ProductCatalog"),
               new ServicePartitionKey(0));
        }

        protected override IEnumerable<ServiceReplicaListener> CreateServiceReplicaListeners()
        {
            return this.CreateServiceRemotingReplicaListeners();
        }

    }
}
