using System;

namespace Belatrix.API.Models
{
    public class ApiBasketAddRequest
    {        
        public Guid ProductId { get; set; }                
        public int Quantity { get; set; }
    }
}
