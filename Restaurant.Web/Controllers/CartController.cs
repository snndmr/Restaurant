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
        private readonly ICouponService _couponService;

        public CartController(ICartService cartService, IProductService productService, ICouponService couponService)
        {
            _cartService = cartService;
            _productService = productService;
            _couponService = couponService;
        }

        public async Task<IActionResult> CartIndex()
        {
            return View(await LoadCartDtoBasedOnLoggedInUser());
        }

        [HttpPost]
        [ActionName("ApplyCoupon")]
        public async Task<IActionResult> ApplyCoupon(CartDto cartDto)
        {
            string? userId = User.Claims.Where(q => q.Type == "sub").FirstOrDefault()?.Value;
            string? accessToken = await HttpContext.GetTokenAsync("access_token");

            if (string.IsNullOrEmpty(userId) || string.IsNullOrEmpty(accessToken))
            {
                return RedirectToAction(nameof(CartIndex));
            }

            ResponseDto response = await _cartService.ApplyCouponAsync<ResponseDto>(cartDto, accessToken);

            if (response != null && response.IsSuccess)
            {
                return RedirectToAction(nameof(CartIndex));
            }

            return View();
        }

        [HttpPost]
        [ActionName("RemoveCoupon")]
        public async Task<IActionResult> RemoveCoupon(CartDto cartDto)
        {
            string? userId = User.Claims.Where(q => q.Type == "sub").FirstOrDefault()?.Value;
            string? accessToken = await HttpContext.GetTokenAsync("access_token");

            if (string.IsNullOrEmpty(userId) || string.IsNullOrEmpty(accessToken))
            {
                return RedirectToAction(nameof(CartIndex));
            }

            ResponseDto response = await _cartService.RemoveCouponAsync<ResponseDto>(userId, accessToken);

            if (response != null && response.IsSuccess)
            {
                return RedirectToAction(nameof(CartIndex));
            }

            return View();
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

        public async Task<IActionResult> Checkout()
        {
            return View(await LoadCartDtoBasedOnLoggedInUser());
        }

        [HttpPost]
        public async Task<IActionResult> Checkout(CartDto cartDto)
        {
            try
            {
                string? accessToken = await HttpContext.GetTokenAsync("access_token");

                if (string.IsNullOrEmpty(accessToken))
                {
                    return View();
                }

                ResponseDto response = await _cartService.CheckoutAsync<ResponseDto>(cartDto.CartHeader, accessToken);

                return RedirectToAction(nameof(Confirmation));
            }
            catch (Exception)
            {
                return View();
            }
        }


        public IActionResult Confirmation()
        {
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
                if (!string.IsNullOrEmpty(cartDto.CartHeader.CouponCode))
                {
                    ResponseDto couponResponse = await _couponService.GetCoupon<ResponseDto>(cartDto.CartHeader.CouponCode, accessToken);

                    if (couponResponse != null && couponResponse.IsSuccess)
                    {
                        CouponDto couponDto = JsonConvert.DeserializeObject<CouponDto>(Convert.ToString(couponResponse.Result));
                        cartDto.CartHeader.DiscountTotal = couponDto.DiscountAmount;
                    }
                }

                foreach (CartDetailDto cartDetail in cartDto.CartDetails)
                {
                    cartDto.CartHeader.OrderTotal += cartDetail.Product.Price * cartDetail.Count;
                }

                cartDto.CartHeader.OrderTotal -= cartDto.CartHeader.DiscountTotal;
            }

            return cartDto;
        }
    }
}
