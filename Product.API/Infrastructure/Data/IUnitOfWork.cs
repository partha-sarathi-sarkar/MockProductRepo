using Microsoft.EntityFrameworkCore;
using System;

namespace Product.API.Infrastructure.Data
{
    public interface IUnitOfWork : IDisposable
    {
        void SaveChanges();
    }

    public interface IUnitOfWork<TContext> : IUnitOfWork where TContext : DbContext
    {
        TContext Context { get; }

        Repository<Model.Product> ProductRepository { get; }
    }

}
