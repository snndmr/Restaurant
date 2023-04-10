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

        public async Task<bool> ApplyCoupon(string userId, string code)
        {
            CartHeader header = await _dbContext.CartHeaders.FirstOrDefaultAsync(q => q.UserId == userId);

            if (header == null) { return false; }

            header.CouponCode = code;

            _dbContext.CartHeaders.Update(header);
            await _dbContext.SaveChangesAsync();

            return true;
        }

        public async Task<bool> ClearCart(string userId)
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

            Product product = _dbContext.Products.FirstOrDefault(q => q.Id == cartDto.CartDetails.FirstOrDefault().ProductId);

            if (product == null)
            {
                _dbContext.Products.Add(cart.CartDetails.First().Product);
                await _dbContext.SaveChangesAsync();
            }

            CartHeader cartHeader = _dbContext.CartHeaders.AsNoTracking().FirstOrDefault(q => q.UserId == cart.CartHeader.UserId);

            if (cartHeader == null)
            {
                _dbContext.CartHeaders.Add(cart.CartHeader);
                await _dbContext.SaveChangesAsync();
                cart.CartDetails.FirstOrDefault().CartHeaderId = cart.CartHeader.Id;
                cart.CartDetails.FirstOrDefault().Product = null;
                _dbContext.CartDetails.Add(cart.CartDetails.FirstOrDefault());
                await _dbContext.SaveChangesAsync();
            }
            else
            {
                CartDetail cartDetail= _dbContext.CartDetails.AsNoTracking().FirstOrDefault(q =>
                q.ProductId == cart.CartDetails.FirstOrDefault().ProductId && q.CartHeaderId == cartHeader.Id);

                if (cartDetail == null)
                {
                    cart.CartDetails.FirstOrDefault().CartHeaderId = cartHeader.Id;
                    cart.CartDetails.FirstOrDefault().Product = null;
                    _dbContext.CartDetails.Add(cart.CartDetails.FirstOrDefault());
                    await _dbContext.SaveChangesAsync();
                }
                else
                {
                    cart.CartDetails.FirstOrDefault().Id = cartDetail.Id;
                    cart.CartDetails.FirstOrDefault().CartHeaderId = cartHeader.Id;
                    cart.CartDetails.FirstOrDefault().Product = null;
                    cart.CartDetails.FirstOrDefault().Count += cartDetail.Count;
                    _dbContext.CartDetails.Update(cart.CartDetails.FirstOrDefault());
                    await _dbContext.SaveChangesAsync();
                }
            }

            return _mapper.Map<CartDto>(cart);
        }

        public async Task<CartDto> GetCartByUserId(string userId)
        {
            Cart cart = new()
            {
                CartHeader = await _dbContext.CartHeaders.FirstOrDefaultAsync(x => x.UserId == userId)
            };

            if (cart.CartHeader != null)
            {
                cart.CartDetails = _dbContext.CartDetails.Where(x => x.CartHeaderId.Equals(cart.CartHeader.Id)).Include(x => x.Product);
            }

            return _mapper.Map<CartDto>(cart);
        }

        public async Task<bool> RemoveCoupon(string userId)
        {
            CartHeader header = await _dbContext.CartHeaders.FirstOrDefaultAsync(q => q.UserId == userId);

            if (header == null) { return false; }

            header.CouponCode = string.Empty;

            _dbContext.CartHeaders.Update(header);
            await _dbContext.SaveChangesAsync();

            return true;
        }

        public async Task<bool> RemoveFromCart(int cartDetailId)
        {
            try
            {
                CartDetail cartDetail = await _dbContext.CartDetails.FirstOrDefaultAsync(x => x.Id.Equals(cartDetailId));

                int totalCountOfCartItems = _dbContext.CartDetails.Where(x => x.CartHeaderId.Equals(cartDetail.CartHeaderId)).Count();

                _dbContext.CartDetails.Remove(cartDetail);

                if (totalCountOfCartItems == 1)
                {
                    CartHeader header = await _dbContext.CartHeaders.FirstOrDefaultAsync(x => x.Id == cartDetail.CartHeaderId);
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

