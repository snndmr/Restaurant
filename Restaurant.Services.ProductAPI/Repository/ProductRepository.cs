using AutoMapper;
using Microsoft.EntityFrameworkCore;
using Restaurant.Services.ProductAPI.DbContexts;
using Restaurant.Services.ProductAPI.Models;
using Restaurant.Services.ProductAPI.Models.Dtos;

namespace Restaurant.Services.ProductAPI.Repository
{
    public class ProductRepository : IProductRepository
    {
        private readonly ApplicationDbContext _dbContext;
        private readonly IMapper _mapper;

        public ProductRepository(ApplicationDbContext dbContext, IMapper mapper)
        {
            _dbContext = dbContext;
            _mapper = mapper;
        }

        public async Task<ProductDto> CreateAndUpdateProduct(ProductDto productDto)
        {
            Product? product = _mapper.Map<Product>(productDto);

            if (product.Id > 0)
            {
                _dbContext.Update(product);
            }
            else
            {
                _dbContext.Add(product);
            }

            await _dbContext.SaveChangesAsync();

            return _mapper.Map<ProductDto>(product);
        }

        public async Task<bool> DeleteProduct(int id)
        {
            try
            {
                Product? product = await _dbContext.Products.Where(x => x.Id.Equals(id)).FirstOrDefaultAsync();

                if (product != null)
                {
                    _dbContext.Remove(product);
                    await _dbContext.SaveChangesAsync();
                }

                return true;
            }
            catch (Exception)
            {
                return false;
            }
        }

        public async Task<ProductDto> GetProductDtoById(int id)
        {
            Product? product = await _dbContext.Products.Where(x => x.Id.Equals(id)).FirstOrDefaultAsync();
            return _mapper.Map<ProductDto>(product);
        }

        public async Task<IEnumerable<ProductDto>> GetProductDtos()
        {
            List<Product> products  = await _dbContext.Products.ToListAsync();
            return _mapper.Map<List<ProductDto>>(products);
        }
    }
}
