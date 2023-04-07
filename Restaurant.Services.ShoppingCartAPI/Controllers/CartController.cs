using Microsoft.AspNetCore.Mvc;
using Restaurant.Services.ShoppingCartAPI.Messages;
using Restaurant.Services.ShoppingCartAPI.Models.Dtos;
using Restaurant.Services.ShoppingCartAPI.Repository;

namespace Restaurant.Services.ShoppingCartAPI.Controllers
{
    [ApiController]
    [Route("api/cart")]
    public class CartController : Controller
    {
        private readonly ICartRepository _cartRepository;
        private readonly ResponseDto _responseDto;

        public CartController(ICartRepository cartRepository)
        {
            _cartRepository = cartRepository;
            _responseDto = new ResponseDto();
        }

        [HttpGet("GetCart/{userId}")]
        public async Task<object> GetCartAsync(string userId)
        {
            try
            {
                _responseDto.Result = await _cartRepository.GetCartByUserId(userId);
            }
            catch (Exception ex)
            {
                _responseDto.IsSuccess = false;
                _responseDto.ErrorMessages = new List<string>() { ex.ToString() };
            }

            return _responseDto;
        }

        [HttpPost("AddCart")]
        public async Task<object> AddCart([FromBody] CartDto cartDto)
        {
            try
            {
                _responseDto.Result = await _cartRepository.CreateUpdateCart(cartDto);
            }
            catch (Exception ex)
            {
                _responseDto.IsSuccess = false;
                _responseDto.ErrorMessages = new List<string>() { ex.ToString() };
            }

            return _responseDto;
        }

        [HttpPost("UpdateCart")]
        public async Task<object> UpdateCart([FromBody] CartDto cartDto)
        {
            try
            {
                _responseDto.Result = await _cartRepository.CreateUpdateCart(cartDto);
            }
            catch (Exception ex)
            {
                _responseDto.IsSuccess = false;
                _responseDto.ErrorMessages = new List<string>() { ex.ToString() };
            }

            return _responseDto;
        }

        [HttpPost("RemoveCart")]
        public async Task<object> RemoveCart([FromBody] int cartId)
        {
            try
            {
                _responseDto.Result = await _cartRepository.RemoveFromCart(cartId);
            }
            catch (Exception ex)
            {
                _responseDto.IsSuccess = false;
                _responseDto.ErrorMessages = new List<string>() { ex.ToString() };
            }

            return _responseDto;
        }

        [HttpPost("ApplyCoupon")]
        public async Task<object> ApplyCoupon([FromBody] CartDto cartDto)
        {
            try
            {
                _responseDto.Result = await _cartRepository.ApplyCoupon(cartDto.CartHeader.UserId, cartDto.CartHeader.CouponCode);
            }
            catch (Exception ex)
            {
                _responseDto.IsSuccess = false;
                _responseDto.ErrorMessages = new List<string>() { ex.ToString() };
            }

            return _responseDto;
        }

        [HttpPost("RemoveCoupon")]
        public async Task<object> RemoveCoupon([FromBody] string userId)
        {
            try
            {
                _responseDto.Result = await _cartRepository.RemoveCoupon(userId);
            }
            catch (Exception ex)
            {
                _responseDto.IsSuccess = false;
                _responseDto.ErrorMessages = new List<string>() { ex.ToString() };
            }

            return _responseDto;
        }

        [HttpPost("Checkout")]
        public async Task<object> Checkout(CheckoutHeaderDto checkoutHeaderDto)
        {
            try
            {
                CartDto cartDto = await _cartRepository.GetCartByUserId(checkoutHeaderDto.UserId);

                if (cartDto == null)
                {
                    return BadRequest();
                }

                checkoutHeaderDto.CartDetails = cartDto.CartDetails;
            }
            catch (Exception ex)
            {
                _responseDto.IsSuccess = false;
                _responseDto.ErrorMessages = new List<string>() { ex.ToString() };
            }

            return _responseDto;
        }
    }
}
