using AutoMapper;
using Microsoft.EntityFrameworkCore;
using Restaurant.Services.CouponAPI.DbContexts;
using Restaurant.Services.CouponAPI.Models;

namespace Restaurant.Services.CouponAPI.Repository
{
    public class CouponRepository : ICouponRepository
    {
        private readonly IMapper _mapper;
        private readonly ApplicationDbContext _db;

        public CouponRepository(IMapper mapper, ApplicationDbContext applicationDbContext)
        {
            _mapper = mapper;
            _db = applicationDbContext;
        }

        public async Task<CouponDto> GetCouponDtoByCode(string code)
        {
            Coupon coupon = await _db.Coupons.Where(q => q.Code == code).FirstOrDefaultAsync();
            return _mapper.Map<CouponDto>(coupon);
        }
    }
}