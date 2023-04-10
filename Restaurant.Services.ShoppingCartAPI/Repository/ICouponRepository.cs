using Restaurant.Services.ShoppingCartAPI.Models.Dtos;

namespace Restaurant.Services.ShoppingCartAPI.Repository
{
    public interface ICouponRepository
    {
        Task<CouponDto> GetCoupon(string couponName);
    }
}
