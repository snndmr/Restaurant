using Restaurant.Repository;
using Restaurant.Services.Email.DbContexts;
using Restaurant.Services.Email.Messages;
using Restaurant.Services.Email.Models;

namespace Restaurant.Services.Email.Repository
{
    public class EmailRepository : IEmailRepository
    {
        private readonly ApplicationDbContext _dbContext;

        public EmailRepository(ApplicationDbContext dbContext)
        {
            _dbContext = dbContext;
        }

        public async Task SendAndLogEmail(UpdatePaymentResultMessage message)
        {
            EmailLog emailLog = new()
            {
                Email = message.Email,
                EmailSent = DateTime.Now,
                Log = $"Order - {message.OrderId} has been created sucessfully"
            };

            _dbContext.EmailLogs.Add(emailLog);
            await _dbContext.SaveChangesAsync();
        }
    }
}
