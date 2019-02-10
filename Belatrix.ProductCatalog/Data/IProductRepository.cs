using Belatrix.ProductCatalog.Models;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Belatrix.ProductCatalog.Data
{
    interface IProductRepository
    {
        Task<IEnumerable<Product>> GetAllProducts();

        Task AddProduct(Product product);
    }
}
