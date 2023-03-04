using Restaurant.Services.ProductAPI.Models.Dtos;

namespace Restaurant.Services.ProductAPI.Repository
{
    public interface IProductRepository
    {
        Task<IEnumerable<ProductDto>> GetProductDtos();
        Task<ProductDto> GetProductDtoById(int id);
        Task<ProductDto> CreateAndUpdateProduct(ProductDto productDto);
        Task<bool> DeleteProduct(int id);
    }
}
