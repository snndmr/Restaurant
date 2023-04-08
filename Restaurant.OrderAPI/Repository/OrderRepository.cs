using Microsoft.EntityFrameworkCore;
using Restaurant.OrderAPI.DbContexts;
using Restaurant.OrderAPI.Models;
using Restaurant.Repository;

namespace Restaurant.OrderAPI.Repository
{
    public class OrderRepository : IOrderRepository
    {
        private readonly DbContextOptions<ApplicationDbContext> _dbContext;

        public OrderRepository(DbContextOptions<ApplicationDbContext> dbContext)
        {
            _dbContext = dbContext;
        }

        public async Task<bool> AddOrder(OrderHeader orderHead)
        {
            await using ApplicationDbContext _db = new(_dbContext);

            _db.OrderHeaders.Add(orderHead);
            await _db.SaveChangesAsync();

            return true;
        }

        public async Task<bool> UpdateOrderPaymentStatus(int orderHeaderId, bool isPaid)
        {
            await using ApplicationDbContext _db = new(_dbContext);

            OrderHeader orderHeader = await _db.OrderHeaders.FirstOrDefaultAsync(q => q.OrderHeaderId == orderHeaderId);

            if (orderHeader != null)
            {
                orderHeader.PaymentStatus = isPaid;
                await _db.SaveChangesAsync();

                return true;
            }

            return false;
        }
    }
}
