﻿using Microsoft.ServiceFabric.Services.Remoting;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Belatrix.CheckoutService.Models
{
    public interface ICheckoutService : IService
    {
        Task<CheckoutSummary> Checkout(string userId);
        Task<IEnumerable<CheckoutSummary>> GetOrderHistory(string userId);
    }
}
