using Restaurant.Web.Models;

namespace Restaurant.Web.Services.IServices
{
    public interface ICartService
    {
        Task<T> GetCartByUserIdAsync<T>(string userId, string accessToken);
        Task<T> AddToCartAsync<T>(CartDto cartDto, string accessToken);
        Task<T> UpdateCartAsync<T>(CartDto cartDto, string accessToken);
        Task<T> RemoveCartAsync<T>(int cartId, string accessToken);
        Task<T> ApplyCouponAsync<T>(CartDto cartDto, string accessToken);
        Task<T> RemoveCouponAsync<T>(string userId, string accessToken);
    }
}
