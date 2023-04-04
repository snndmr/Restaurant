using Microsoft.EntityFrameworkCore;
using Restaurant.Services.CouponAPI.Models;

namespace Restaurant.Services.CouponAPI.DbContexts
{
    public class ApplicationDbContext : DbContext
    {
        public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options) : base(options) { }

        public DbSet<Coupon> Coupons { get; set; }
    }
}
