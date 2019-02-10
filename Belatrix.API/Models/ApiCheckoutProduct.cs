using System;

namespace Belatrix.API.Models
{
    public class ApiCheckoutProduct
   {
      public Guid ProductId { get; set; }
      public string ProductName { get; set; }
      public int Quantity { get; set; }
      public double Price { get; set; }
   }
}