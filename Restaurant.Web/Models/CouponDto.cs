﻿namespace Restaurant.Web.Models
{
    public class CouponDto
    {
        public int Id { get; set; }

        public string? Code { get; set; }

        public double DiscountAmount { get; set; }
    }
}
