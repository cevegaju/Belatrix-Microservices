﻿namespace Belatrix.API.Models
{
    public class ApiBasket
    {        
        public string UserId { get; set; }
        public ApiBasketItem[] Items { get; set; }
    }

    public class ApiBasketItem
    {
        public string ProductId { get; set; }
        public int Quantity { get; set; }
    }
}
