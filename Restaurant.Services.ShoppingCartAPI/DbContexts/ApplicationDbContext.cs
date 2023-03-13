using Microsoft.EntityFrameworkCore;
using Restaurant.Services.ShoppingCartAPI.Models;

namespace Restaurant.Services.ShoppingCartAPI.DbContexts
{
    public class ApplicationDbContext : DbContext
    {
        public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options) : base(options) { }

        public DbSet<CartDetail> CartDetails { get; set; }
        public DbSet<CartHeader> CartHeaders { get; set; }
        public DbSet<Product> Products { get; set; }
    }
}
