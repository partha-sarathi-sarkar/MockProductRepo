using Product.API.Infrastructure.Data;
using System;
using System.Collections.Generic;
using System.Linq;

namespace Product.API.Infrastructure.Services
{
    public class ProductService : IProductService
    {
        private readonly IUnitOfWork<ProductContext> _unitOfWork;

        public ProductService(IUnitOfWork<ProductContext> unitOfWork)
        {
            _unitOfWork = unitOfWork;
        }

        public bool AddProduct(Model.Product product)
        {
            try
            {
                _unitOfWork.ProductRepository.Add(product);
                _unitOfWork.SaveChanges();
                return true;
            }
            catch (Exception)
            {
                return false;
            }
        }

        public Model.Product GetProduct(int productId)
        {
            return _unitOfWork.ProductRepository.Get(p => p.ProductID == productId).FirstOrDefault();
        }

        public IEnumerable<Model.Product> GetProducts()
        {
            return _unitOfWork.ProductRepository.Get();
        }

        public bool RemoveProduct(int productId)
        {
            try
            {
                _unitOfWork.ProductRepository.Delete(productId);
                _unitOfWork.SaveChanges();
                return true;
            }
            catch(Exception)
            {
                return false;
            }
        }

        public bool UpdateProduct(Model.Product product)
        {
            try
            {
                _unitOfWork.ProductRepository.Update(product);
                _unitOfWork.SaveChanges();
                return true;
            }
            catch (Exception)
            {
                return false;
            }
        }
    }
}
