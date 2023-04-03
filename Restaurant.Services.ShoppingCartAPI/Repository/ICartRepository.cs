﻿using Restaurant.Services.ShoppingCartAPI.Models.Dtos;

namespace Restaurant.Services.ShoppingCartAPI.Repository
{
    public interface ICartRepository
    {
        Task<CartDto> GetCartByUserId(string userId);
        Task<CartDto> CreateUpdateCart(CartDto cartDto);
        Task<bool> RemoveFromCart(int cartDetailId);
        Task<bool> ClearCart(string userId);
    }
}
