using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;
using Restaurant.Web.Models;
using Restaurant.Web.Services.IServices;
using System.Diagnostics;

namespace Restaurant.Web.Controllers
{
    public class HomeController : Controller
    {
        private readonly ILogger<HomeController> _logger;
        private readonly IProductService _productService;
        private readonly ICartService _cartService;

        public HomeController(ILogger<HomeController> logger, IProductService productService, ICartService cartService)
        {
            _logger = logger;
            _productService = productService;
            _cartService = cartService;
        }

        public async Task<IActionResult> Index()
        {
            ResponseDto response = await _productService.GetAllProductsAsync<ResponseDto>(string.Empty);

            if (response == null || !response.IsSuccess)
            {
                return NoContent();
            }

            string? result = Convert.ToString(response.Result);

            if (result == null)
            {
                return NoContent();
            }

            return View(JsonConvert.DeserializeObject<List<ProductDto>>(result));
        }

        [Authorize]
        public async Task<IActionResult> Details(int productId)
        {
            string? accessToken = await HttpContext.GetTokenAsync("access_token");

            if (accessToken == null)
            {
                return Unauthorized();
            }

            ResponseDto response = await _productService.GetProductByIdAsync<ResponseDto>(productId, accessToken);

            if (response == null || !response.IsSuccess)
            {
                return NoContent();
            }

            string? result = Convert.ToString(response.Result);

            if (result == null)
            {
                return NoContent();
            }

            return View(JsonConvert.DeserializeObject<ProductDto>(result));
        }

        [Authorize]
        [HttpPost]
        [ActionName("Details")]
        public async Task<IActionResult> DetailsPost(ProductDto productDto)
        {
            string? accessToken = await HttpContext.GetTokenAsync("access_token");

            CartDetailDto cartDetailDto = new()
            {
                Count = productDto.Count,
                ProductId = productDto.Id,
                CartHeader = new() { UserId = User.Claims.Where(x => x.Type.Equals("sub"))?.FirstOrDefault()?.Value },
            };

            ResponseDto responseDto = await _productService.GetProductByIdAsync<ResponseDto>(productDto.Id, accessToken ?? string.Empty);

            if (responseDto?.IsSuccess == true)
            {
                cartDetailDto.Product = JsonConvert.DeserializeObject<ProductDto>(Convert.ToString(responseDto.Result));
            }

            CartDto cartDto = new()
            {
                CartHeader = new() { UserId = User.Claims.Where(x => x.Type.Equals("sub"))?.FirstOrDefault()?.Value },
                CartDetails = new List<CartDetailDto>() { cartDetailDto }
            };

            var addToCartResponseDto = await _cartService.AddToCartAsync<ResponseDto>(cartDto, accessToken ?? string.Empty);

            if (responseDto?.IsSuccess == true)
            {
                return RedirectToAction(nameof(Index));
            }

            return View(productDto);
        }

        [Authorize]
        public IActionResult Login()
        {
            return RedirectToAction(nameof(Index));
        }

        public IActionResult Logout()
        {
            return SignOut("Cookies", "oidc");
        }

        public IActionResult Privacy()
        {
            return View();
        }

        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Error()
        {
            return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
        }
    }
}