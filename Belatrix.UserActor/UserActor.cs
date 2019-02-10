using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Microsoft.ServiceFabric.Actors;
using Microsoft.ServiceFabric.Actors.Runtime;
using Belatrix.UserActor.Interfaces;

namespace Belatrix.UserActor
{
    [StatePersistence(StatePersistence.Persisted)]
    internal class UserActor : Actor, IUserActor
    {        
        public UserActor(ActorService actorService, ActorId actorId)
            : base(actorService, actorId)
        {
        }

        public async Task AddToBasket(Guid productId, int quantity)
        {
            await StateManager.AddOrUpdateStateAsync(productId.ToString(),
                quantity,
                (id, oldQuantity) => oldQuantity + quantity);
        }

        public async Task ClearBasket()
        {
            IEnumerable<string> productIDs = await StateManager.GetStateNamesAsync();

            foreach (string productId in productIDs)
            {
                await StateManager.RemoveStateAsync(productId);
            }

        }

        public async Task<Dictionary<Guid, int>> GetBasket()
        {
            var result = new Dictionary<Guid, int>();

            IEnumerable<string> productIDs = await StateManager.GetStateNamesAsync();

            foreach (string productId in productIDs)
            {
                int quantity = await StateManager.GetStateAsync<int>(productId);
                result[new Guid(productId)] = quantity;
            }

            return result;
        }
    }
}
