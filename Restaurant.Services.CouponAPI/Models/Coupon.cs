using System.ComponentModel.DataAnnotations;

namespace Restaurant.Services.CouponAPI.Models
{
    public class Coupon
    {
        [Key]
        public int Id { get; set; }

        public string Code { get; set; }

        public double DiscountAmount { get; set; }
    }
}
