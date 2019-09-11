using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Npgsql;
using System;
using System.Data.Common;

namespace Product.API.Infrastructure.Data
{
    public class Postgres : IDatabaseType
    {
        public IServiceCollection EnableDatabase(IServiceCollection services)
        {
            services.AddEntityFrameworkNpgsql();
            return services;
        }

        public DbConnectionStringBuilder GetConnectionBuilder(string connectionString)
        {
            return new NpgsqlConnectionStringBuilder(connectionString);
        }

        public DbContextOptionsBuilder GetContextBuilder(DbContextOptionsBuilder optionsBuilder,
            string connectionString)
        {
            return optionsBuilder.UseNpgsql(connectionString,
                options => { options.EnableRetryOnFailure(5, TimeSpan.FromSeconds(30), null); });
            //return optionsBuilder.UseNpgsql(connectionString, b => EntityFrameworkConfiguration.GetMigrationInformation(b));
        }

        public DbContextOptionsBuilder SetConnectionString(DbContextOptionsBuilder contextOptionsBuilder,
            string connectionString)
        {
            return contextOptionsBuilder.UseNpgsql(connectionString,
                options => { options.EnableRetryOnFailure(5, TimeSpan.FromSeconds(30), null); });
        }
    }
}
