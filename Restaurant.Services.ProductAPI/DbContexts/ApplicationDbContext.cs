using Microsoft.EntityFrameworkCore;
using Restaurant.Services.ProductAPI.Models;

namespace Restaurant.Services.ProductAPI.DbContexts
{
    public class ApplicationDbContext : DbContext
    {
        public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options) : base(options) { }

        public DbSet<Product> Products { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            modelBuilder.Entity<Product>().HasData(
                new Product
                {
                    Id = 1,
                    Name = "Jam - Apricot",
                    Price = 5,
                    Description = "Proin leo odio, porttitor id, consequat in, consequat ut, nulla. Sed accumsan felis. Ut at dolor quis odio consequat varius. Integer ac leo. Pellentesque ultrices mattis odio. Donec vitae nisi. Nam ultrices, libero non mattis pulvinar, nulla pede ullamcorper augue, a suscipit nulla elit ac nulla. Sed vel enim sit amet nunc viverra dapibus.",
                    ImageUrl = "http://dummyimage.com/242x100.png/5fa2dd/ffffff",
                    CategoryName = "Plumbing & Medical Gas"
                },
                new Product
                {
                    Id = 2,
                    Name = "Soup - Campbellschix Stew",
                    Price = 37,
                    Description = "Aliquam non mauris.",
                    ImageUrl = "http://dummyimage.com/159x100.png/5fa2dd/ffffff",
                    CategoryName = "Drywall & Acoustical (FED)"
                },
                new Product
                {
                    Id = 3,
                    Name = "Swiss Chard - Red",
                    Price = 91,
                    Description = "Lorem ipsum dolor sit amet, consectetuer adipiscing elit. Proin risus. Praesent lectus. Vestibulum quam sapien, varius ut, blandit non, interdum in, ante. Vestibulum ante ipsum primis in faucibus orci luctus et ultrices posuere cubilia Curae; Duis faucibus accumsan odio.",
                    ImageUrl = "http://dummyimage.com/150x100.png/ff4444/ffffff",
                    CategoryName = "HVAC"
                },
                new Product
                {
                    Id = 4,
                    Name = "Ham - Cooked Italian",
                    Price = 81,
                    Description = "Aenean fermentum. Donec ut mauris eget massa tempor convallis. Nulla neque libero, convallis eget, eleifend luctus, ultricies eu, nibh. Quisque id justo sit amet sapien dignissim vestibulum. Vestibulum ante ipsum primis in faucibus orci luctus et ultrices posuere cubilia Curae; Nulla dapibus dolor vel est. Donec odio justo, sollicitudin ut, suscipit a, feugiat et, eros. Vestibulum ac est lacinia nisi venenatis tristique. Fusce congue, diam id ornare imperdiet, sapien urna pretium nisl, ut volutpat sapien arcu sed augue. Aliquam erat volutpat.",
                    ImageUrl = "http://dummyimage.com/164x100.png/cc0000/ffffff",
                    CategoryName = "Glass & Glazing"
                },
                new Product
                {
                    Id = 5,
                    Name = "Mushroom - Portebello",
                    Price = 24,
                    Description = "Duis ac nibh. Fusce lacus purus, aliquet at, feugiat non, pretium quis, lectus. Suspendisse potenti. In eleifend quam a odio.",
                    ImageUrl = "http://dummyimage.com/185x100.png/ff4444/ffffff",
                    CategoryName = "Fire Sprinkler System"
                }
            );
        }
    }
}
