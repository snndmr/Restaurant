using AutoMapper;
using Microsoft.EntityFrameworkCore;
using Restaurant.Services.ShoppingCartAPI.DbContexts;
using Restaurant.Services.ShoppingCartAPI.Models;
using Restaurant.Services.ShoppingCartAPI.Models.Dtos;

namespace Restaurant.Services.ShoppingCartAPI.Repository
{
    public class CartRepository : ICartRepository
    {
        private readonly ApplicationDbContext _dbContext;
        private readonly IMapper _mapper;

        public CartRepository(ApplicationDbContext dbContext, IMapper mapper)
        {
            _dbContext = dbContext;
            _mapper = mapper;
        }

        public async Task<bool> ClearCart(int userId)
        {
            CartHeader header = await _dbContext.CartHeaders.FirstAsync(x => x.UserId == userId);

            if (header != null)
            {
                _dbContext.CartDetails.RemoveRange(_dbContext.CartDetails.Where(x => x.CartHeaderId.Equals(header.Id)));
                _dbContext.CartHeaders.Remove(header);
                await _dbContext.SaveChangesAsync();

                return true;
            }

            return false;
        }

        public async Task<CartDto> CreateUpdateCart(CartDto cartDto)
        {
            Cart cart = _mapper.Map<Cart>(cartDto);

            if (cart == null)
            {
                return cartDto;
            }

            Product product = _dbContext.Products.First(q => q .Id == cartDto.CartDetails.First().ProductId);

            if (product == null)
            {
                _dbContext.Products.Add(cart.CartDetails.First().Product);
                await _dbContext.SaveChangesAsync();
            }

            CartHeader cartHeader = _dbContext.CartHeaders.AsNoTracking().First(q => q.UserId == cart.CartHeader.UserId);

            if (cartHeader == null)
            {
                _dbContext.CartHeaders.Add(cart.CartHeader);
                await _dbContext.SaveChangesAsync();
                cart.CartDetails.First().CartHeaderId = cart.CartHeader.Id;
                cart.CartDetails.First().Product = null;
                _dbContext.CartDetails.Add(cart.CartDetails.First());
                await _dbContext.SaveChangesAsync();
            }
            else
            {
                CartDetail cartDetail= _dbContext.CartDetails.AsNoTracking().First(q =>
                q.ProductId == cart.CartDetails.First().ProductId && q.CartHeaderId == cartHeader.Id);

                if (cartDetail == null)
                {
                    cart.CartDetails.First().CartHeaderId = cartHeader.Id;
                    cart.CartDetails.First().Product = null;
                    _dbContext.CartDetails.Add(cart.CartDetails.First());
                    await _dbContext.SaveChangesAsync();
                }
                else
                {
                    cart.CartDetails.First().Product = null;
                    cart.CartDetails.First().Count += cartDetail.Count;
                    _dbContext.CartDetails.Update(cart.CartDetails.First());
                    await _dbContext.SaveChangesAsync();
                }
            }

            return _mapper.Map<CartDto>(cart);
        }

        public async Task<CartDto> GetCartByUserId(int userId)
        {
            Cart cart = new()
            {
                CartHeader = await _dbContext.CartHeaders.FirstAsync(x => x.UserId == userId)
            };

            cart.CartDetails = _dbContext.CartDetails.Where(x => x.CartHeaderId.Equals(cart.CartHeader.Id)).Include(x => x.Product);

            return _mapper.Map<CartDto>(cart);
        }

        public async Task<bool> RemoveFromCart(int cartDetailId)
        {
            try
            {
                CartDetail cartDetail = await _dbContext.CartDetails.FirstAsync(x => x.Id.Equals(cartDetailId));

                int totalCountOfCartItems = _dbContext.CartDetails.Where(x => x.CartHeaderId.Equals(cartDetail.CartHeaderId)).Count();

                _dbContext.CartDetails.Remove(cartDetail);

                if (totalCountOfCartItems == 1)
                {
                    CartHeader header = await _dbContext.CartHeaders.FirstAsync(x => x.Id == cartDetail.CartHeaderId);
                    _dbContext.CartHeaders.Remove(header);
                }

                await _dbContext.SaveChangesAsync();
                return true;
            }
            catch (Exception)
            {
                return false;
            }

        }
    }
}

