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

        public HomeController(ILogger<HomeController> logger, IProductService productService)
        {
            _logger = logger;
            _productService = productService;
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
        public async Task<IActionResult> Login()
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