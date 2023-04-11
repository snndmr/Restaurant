﻿namespace Restaurant.Services.OrderAPI.Messages
{
    public class CartDetailDto
    {
        public int Id { get; set; }
        public int CartHeaderId { get; set; }
        public int ProductId { get; set; }
        public virtual ProductDto Product { get; set; }
        public int Count { get; set; }
    }
}
