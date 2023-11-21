using BanPhimCanhCach.Models;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;

namespace BanPhimCanhCach.AppData
{
 public class AppDbContext : IdentityDbContext<AppUser>
 {
        public AppDbContext(DbContextOptions<AppDbContext> options) : base(options) 
        {
            
        }

        protected override void OnModelCreating(ModelBuilder builder)
        {
            base.OnModelCreating(builder);
            foreach(var entityType in builder.Model.GetEntityTypes())
            {
                var tableName = entityType.GetTableName();
                if(tableName.StartsWith("AspNet"))
                {
                    entityType.SetTableName(tableName.Substring(6));
                }
            };

            builder.Entity<Product>(entity =>
            {
                entity.HasMany(p => p.ProductVariants)
                      .WithOne(pc => pc.Product)
                      .OnDelete(DeleteBehavior.SetNull);
                entity.HasMany(p => p.ProductImages)
                      .WithOne(pc => pc.Product)
                      .OnDelete(DeleteBehavior.Cascade);
            });

            builder.Entity<Order>(entity =>
            {
                entity.HasMany(o => o.OrderItems)
                    .WithOne(oi => oi.Order)
                    .OnDelete(DeleteBehavior.Cascade);
            });
        }

        public DbSet<Product> Products {get; set;}
        public DbSet<ProductVariant> ProductVariants { get; set;}
        public DbSet<ProductImage> ProductImages {get;set;}
        public DbSet<Order> Orders { get; set;}
        public DbSet<OrderItem> OrderItems { get; set;}
        public DbSet<Notification> Notifications { get; set;}
    }   
}