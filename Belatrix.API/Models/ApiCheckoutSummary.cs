using System;
using System.Collections.Generic;

namespace Belatrix.API.Models
{
    public class ApiCheckoutSummary
   {
      public List<ApiCheckoutProduct> Products { get; set; }
      public double TotalPrice { get; set; }
      public DateTime Date { get; set; }
   }
}
