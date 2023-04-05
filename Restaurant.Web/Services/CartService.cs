using Restaurant.Web.Models;
using Restaurant.Web.Services.IServices;

namespace Restaurant.Web.Services
{
    public class CartService : BaseService, ICartService
    {
        public CartService(IHttpClientFactory httpClientFactory) : base(httpClientFactory) { }

        public async Task<T> AddToCartAsync<T>(CartDto cartDto, string accessToken)
        {
            return await SendAsync<T>(new ApiRequest()
            {
                ApiType = StaticDetails.APIType.POST,
                Data = cartDto,
                Url = StaticDetails.ShoppingCartAPIBase + "/api/cart/AddCart",
                AccessToken = accessToken,
            });
        }

        public async Task<T> ApplyCouponAsync<T>(CartDto cartDto, string accessToken)
        {
            return await SendAsync<T>(new ApiRequest()
            {
                ApiType = StaticDetails.APIType.POST,
                Data = cartDto,
                Url = StaticDetails.ShoppingCartAPIBase + "/api/cart/ApplyCoupon",
                AccessToken = accessToken,
            });
        }

        public async Task<T> GetCartByUserIdAsync<T>(string userId, string accessToken)
        {
            return await SendAsync<T>(new ApiRequest()
            {
                ApiType = StaticDetails.APIType.GET,
                Url = StaticDetails.ShoppingCartAPIBase + "/api/cart/GetCart/" + userId,
                AccessToken = accessToken,
            });
        }

        public async Task<T> RemoveCartAsync<T>(int cartId, string accessToken)
        {
            return await SendAsync<T>(new ApiRequest()
            {
                ApiType = StaticDetails.APIType.POST,
                Data = cartId,
                Url = StaticDetails.ShoppingCartAPIBase + "/api/cart/RemoveCart",
                AccessToken = accessToken,
            });
        }

        public async Task<T> RemoveCouponAsync<T>(string userId, string accessToken)
        {
            return await SendAsync<T>(new ApiRequest()
            {
                ApiType = StaticDetails.APIType.POST,
                Data = userId,
                Url = StaticDetails.ShoppingCartAPIBase + "/api/cart/RemoveCoupon",
                AccessToken = accessToken,
            });
        }

        public async Task<T> UpdateCartAsync<T>(CartDto cartDto, string accessToken)
        {
            return await SendAsync<T>(new ApiRequest()
            {
                ApiType = StaticDetails.APIType.POST,
                Data = cartDto,
                Url = StaticDetails.ShoppingCartAPIBase + "/api/cart/UpdateCart",
                AccessToken = accessToken,
            });
        }
    }
}
