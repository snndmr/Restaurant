using Microsoft.AspNetCore.Mvc;
using Restaurant.Services.CouponAPI.Models.Dtos;
using Restaurant.Services.CouponAPI.Repository;

namespace Restaurant.Services.CouponAPI.Controllers
{
    [Route("api/[controller]")]
    public class CouponAPIController : ControllerBase
    {
        private readonly ResponseDto _responseDto;
        private readonly ICouponRepository _couponRepository;

        public CouponAPIController(ICouponRepository couponRepository)
        {
            _couponRepository = couponRepository;
            _responseDto = new ResponseDto();
        }

        [HttpGet("{code}")]
        public async Task<object> Get(string code)
        {
            try
            {
                _responseDto.Result = await _couponRepository.GetCouponDtoByCode(code);
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
