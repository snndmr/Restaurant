using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;
using Restaurant.Web.Models;
using Restaurant.Web.Services.IServices;

namespace Restaurant.Web.Controllers
{
    public class ProductController : Controller
    {
        private readonly IProductService _productService;

        public ProductController(IProductService productService) => _productService = productService;

        public async Task<IActionResult> ProductIndex()
        {
            List<ProductDto> list = new();
            ResponseDto response = await _productService.GetAllProductsAsync<ResponseDto>();

            if (response != null && response.IsSuccess)
            {
                list = JsonConvert.DeserializeObject<List<ProductDto>>(Convert.ToString(response.Result));
            }

            return View(list);
        }

        public IActionResult ProductCreate()
        {
            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> ProductCreate(ProductDto productDto)
        {
            if (ModelState.IsValid)
            {
                ResponseDto response = await _productService.CreateProductAsync<ResponseDto>(productDto);

                if (response != null && response.IsSuccess)
                {
                    return RedirectToAction(nameof(ProductIndex));
                }
            }

            return View(productDto);
        }

        public async Task<IActionResult> ProductEdit(int id)
        {
            ResponseDto response = await _productService.GetProductByIdAsync<ResponseDto>(id);

            if (response != null && response.IsSuccess)
            {
                ProductDto productDto = JsonConvert.DeserializeObject<ProductDto>(Convert.ToString(response.Result));
                return View(productDto);
            }

            return NotFound();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> ProductEdit(ProductDto productDto)
        {
            if (ModelState.IsValid)
            {
                ResponseDto response = await _productService.UpdateProductAsync<ResponseDto>(productDto);

                if (response != null && response.IsSuccess)
                {
                    return RedirectToAction(nameof(ProductIndex));
                }
            }

            return View(productDto);
        }

        public async Task<IActionResult> ProductDelete(int id)
        {
            ResponseDto response = await _productService.GetProductByIdAsync<ResponseDto>(id);

            if (response != null && response.IsSuccess)
            {
                ProductDto productDto = JsonConvert.DeserializeObject<ProductDto>(Convert.ToString(response.Result));
                return View(productDto);
            }

            return NotFound();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> ProductDelete(ProductDto productDto)
        {
            if (ModelState.IsValid)
            {
                ResponseDto response = await _productService.DeleteProductAsync<ResponseDto>(productDto.Id);

                if (response != null && response.IsSuccess)
                {
                    return RedirectToAction(nameof(ProductIndex));
                }
            }

            return View(productDto);
        }
    }
}
