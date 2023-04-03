using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;
using Restaurant.Web.Models;
using Restaurant.Web.Services.IServices;

namespace Restaurant.Web.Controllers
{
    public class CartController : Controller
    {
        private readonly ICartService _cartService;
        private readonly IProductService _productService;

        public CartController(ICartService cartService, IProductService productService)
        {
            _cartService = cartService;
            _productService = productService;
        }

        public async Task<IActionResult> CartIndex()
        {
            return View(await LoadCartDtoBasedOnLoggedInUser());
        }

        public async Task<IActionResult> Remove(int cartDetailsId)
        {
            string? userId = User.Claims.Where(q => q.Type == "sub").FirstOrDefault()?.Value;
            string? accessToken = await HttpContext.GetTokenAsync("access_token");

            CartDto cartDto = new();

            if (string.IsNullOrEmpty(userId) || string.IsNullOrEmpty(accessToken))
            {
                return RedirectToAction(nameof(CartIndex));
            }

            ResponseDto response = await _cartService.RemoveCartAsync<ResponseDto>(cartDetailsId, accessToken);

            if (response != null && response.IsSuccess)
            {
                return RedirectToAction(nameof(CartIndex));
            }

            return View();
        }

        private async Task<CartDto> LoadCartDtoBasedOnLoggedInUser()
        {
            string? userId = User.Claims.Where(q => q.Type == "sub").FirstOrDefault()?.Value;
            string? accessToken = await HttpContext.GetTokenAsync("access_token");

            CartDto cartDto = new();

            if (string.IsNullOrEmpty(userId) || string.IsNullOrEmpty(accessToken))
            {
                return cartDto;
            }

            ResponseDto response = await _cartService.GetCartByUserIdAsync<ResponseDto>(userId, accessToken);

            if (response != null && response.IsSuccess)
            {
                cartDto = JsonConvert.DeserializeObject<CartDto>(Convert.ToString(response.Result));
            }

            if (cartDto?.CartHeader != null)
            {
                foreach (CartDetailDto cartDetail in cartDto.CartDetails)
                {
                    cartDto.CartHeader.OrderTotal += cartDetail.Product.Price * cartDetail.Count;
                }
            }

            return cartDto;
        }
    }
}
