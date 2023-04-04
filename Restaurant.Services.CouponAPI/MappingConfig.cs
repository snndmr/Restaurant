using AutoMapper;
using Restaurant.Services.CouponAPI.Models;

namespace Restaurant.Services.CouponAPI
{
    public class MappingConfig
    {
        public static MapperConfiguration RegisterMaps()
        {
            return new(config => config.CreateMap<Coupon, CouponDto>().ReverseMap());
        }
    }
}
