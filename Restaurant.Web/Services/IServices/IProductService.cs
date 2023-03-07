using Restaurant.Web.Models;

namespace Restaurant.Web.Services.IServices
{
    public interface IProductService
    {
        Task<T> GetAllProductsAsync<T>(string accessToken);
        Task<T> GetProductByIdAsync<T>(int id, string accessToken);
        Task<T> CreateProductAsync<T>(ProductDto productDto, string accessToken);
        Task<T> UpdateProductAsync<T>(ProductDto productDto, string accessToken);
        Task<T> DeleteProductAsync<T>(int id, string accessToken);
    }
}
