using Restaurant.Web.Models;
using Restaurant.Web.Services.IServices;

namespace Restaurant.Web.Services
{
    public class CouponService : BaseService, ICouponService
    {
        public CouponService(IHttpClientFactory httpClientFactory) : base(httpClientFactory) { }

        public async Task<T> GetCoupon<T>(string code, string accessToken)
        {
            return await SendAsync<T>(new ApiRequest()
            {
                ApiType = StaticDetails.APIType.GET,
                Url = StaticDetails.CouponAPIBase + "/api/CouponAPI/" + code,
                AccessToken = accessToken,
            });
        }
    }
}
