namespace Restaurant.Web.Models
{
    public class CartHeaderDto
    {
        public int Id { get; set; }

        public string? UserId { get; set; }

        public string? CouponCode { get; set; }

        public double OrderTotal { get; set; }

        public double DiscountTotal { get; set; }

        public string? FirstName { get; set; }

        public string? LastName { get; set; }

        public string? EmailAddress { get; set; }

        public string? CardNumber { get; set; }

        public string? CVV { get; set; }

        public string? ExpiryMonthYear { get; set; }
    }
}