using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Options;
using Product.API.Infrastructure.Data;

namespace Product.API.Infrastructure
{
    public class ProductContext : DbContext
    {

        private readonly IOptions<ConnectionSettings> _connectionOptions;
        public ProductContext(DbContextOptions<ProductContext> options, IOptions<ConnectionSettings> connectionOptions) : base(options)
        {
            _connectionOptions = connectionOptions;
        }

        public DbSet<Model.Product> Products { get; set; }

        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {
            optionsBuilder.UseNpgsql(_connectionOptions.Value.DefaultConnection);
        }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.HasDefaultSchema("public");
            modelBuilder.Entity<Model.Product>(entity =>
            {
                entity.ToTable("Products", "public");
                entity.Property(e => e.ProductID).HasColumnName("ProductID").HasDefaultValueSql("nextval('public.product_id_seq'::regclass)");
                entity.Property(e => e.ProductDescription).HasColumnName("ProductDescription").HasMaxLength(500);
                entity.Property(e => e.ProductName).HasColumnName("ProductName").IsRequired().HasMaxLength(200);
                entity.Property(e => e.Price).HasColumnName("Price").IsRequired();
            });
            modelBuilder.HasSequence("product_id_seq", "public");
        }
    }

    

}
