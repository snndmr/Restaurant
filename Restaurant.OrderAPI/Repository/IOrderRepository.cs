using Restaurant.OrderAPI.Models;

namespace Restaurant.Repository
{
    public interface IOrderRepository
    {
        Task<bool> AddOrder(OrderHeader orderHead);
        Task<bool> UpdateOrderPaymentStatus(int orderHeaderId, bool isPaid);
    }
}
