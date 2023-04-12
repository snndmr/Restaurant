using Microsoft.EntityFrameworkCore;
using Restaurant.Services.Email.Models;

namespace Restaurant.Services.Email.DbContexts
{
    public class ApplicationDbContext : DbContext
    {
        public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options) : base(options)
        {
            AppContext.SetSwitch("Npgsql.EnableLegacyTimestampBehavior", true);
        }

        public DbSet<EmailLog> EmailLogs { get; set; }
    }
}
