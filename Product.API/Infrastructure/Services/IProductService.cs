using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Product.API.Model;

namespace Product.API.Infrastructure.Services
{
    public interface IProductService
    {
        IEnumerable<Model.Product> GetProducts();

        Model.Product GetProduct(int productId);

        bool AddProduct(Model.Product product);

        bool UpdateProduct(Model.Product product);

        bool RemoveProduct(int productId);
    }
}
