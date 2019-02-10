using Belatrix.ProductCatalog.Models;

namespace Belatrix.CheckoutService.Models
{
    public class CheckoutProduct
    {
        public Product Product { get; set; }
        public int Quantity { get; set; }
        public double Price { get; set; }
    }
}
