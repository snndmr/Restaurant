using Newtonsoft.Json;
using Restaurant.Services.ShoppingCartAPI.Models.Dtos;

namespace Restaurant.Services.ShoppingCartAPI.Repository
{
    public class CouponRepository : ICouponRepository
    {
        private readonly HttpClient _client;

        public CouponRepository(HttpClient client) => _client = client;

        public async Task<CouponDto> GetCoupon(string couponName)
        {
            HttpResponseMessage responseMessage = await _client.GetAsync($"/api/CouponAPI/{couponName}");
            string apiContent = await responseMessage.Content.ReadAsStringAsync();
            ResponseDto response = JsonConvert.DeserializeObject<ResponseDto>(apiContent);

            if (response.IsSuccess)
            {
                return JsonConvert.DeserializeObject<CouponDto>(Convert.ToString(response.Result));
            }

            return new();
        }
    }
}
