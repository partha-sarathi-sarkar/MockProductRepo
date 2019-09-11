using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using System;
using System.Collections.Generic;
using System.Data.Common;
using System.Linq;
using System.Threading.Tasks;

namespace Product.API.Infrastructure.Data
{
    public interface IDatabaseType
    {
        IServiceCollection EnableDatabase(IServiceCollection services);
        DbContextOptionsBuilder GetContextBuilder(DbContextOptionsBuilder optionsBuilder, string connectionString);
        DbConnectionStringBuilder GetConnectionBuilder(string connectionString);
        DbContextOptionsBuilder SetConnectionString(DbContextOptionsBuilder contextOptionsBuilder,
            string connectionString);
    }
}
