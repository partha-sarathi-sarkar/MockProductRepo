using Microsoft.EntityFrameworkCore;
using Product.API.Model;
using System;

namespace Product.API.Infrastructure.Data
{
    public class UnitOfWork<TContext> : IUnitOfWork, IUnitOfWork<TContext> where TContext : DbContext
    {
        public TContext Context { get; }
        private Repository<Model.Product> _productRepository;

        public Repository<Model.Product> ProductRepository {
            get
            {
                if(_productRepository == null)
                {
                    _productRepository = new Repository<Model.Product>(Context);
                }
                return _productRepository;
            }
        }        

        public UnitOfWork(TContext context)
        {
            Context = context ?? throw new ArgumentNullException(nameof(context));
        }

        public void Dispose()
        {
            Context?.Dispose();
        }

        public void SaveChanges()
        {
            Context.SaveChanges();
        }
    }
}
