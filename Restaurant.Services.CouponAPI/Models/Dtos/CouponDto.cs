﻿namespace Restaurant.Services.CouponAPI.Models
{
    public class CouponDto
    {
        public int Id { get; set; }

        public string Code { get; set; }

        public double DiscountAmount { get; set; }
    }
}
