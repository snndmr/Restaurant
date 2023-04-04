using Restaurant.Services.CouponAPI.Models;

namespace Restaurant.Services.CouponAPI.Repository
{
    public interface ICouponRepository
    {
        Task<CouponDto> GetCouponDtoByCode(string code);
    }
}
