﻿using Restaurant.Web.Models;
using Restaurant.Web.Services.IServices;

namespace Restaurant.Web.Services
{
    public class ProductService : BaseService, IProductService
    {
        public ProductService(IHttpClientFactory httpClientFactory) : base(httpClientFactory) { }

        public async Task<T> CreateProductAsync<T>(ProductDto productDto, string accessToken)
        {
            return await SendAsync<T>(new ApiRequest()
            {
                ApiType = StaticDetails.APIType.POST,
                Data = productDto,
                Url = StaticDetails.ProductAPIBase + "/api/ProductAPI",
                AccessToken = accessToken,
            });
        }

        public async Task<T> DeleteProductAsync<T>(int id, string accessToken)
        {
            return await SendAsync<T>(new ApiRequest()
            {
                ApiType = StaticDetails.APIType.DELETE,
                Url = StaticDetails.ProductAPIBase + "/api/ProductAPI/" + id,
                AccessToken = accessToken,
            });
        }

        public async Task<T> GetAllProductsAsync<T>(string accessToken)
        {
            return await SendAsync<T>(new ApiRequest()
            {
                ApiType = StaticDetails.APIType.GET,
                Url = StaticDetails.ProductAPIBase + "/api/ProductAPI",
                AccessToken = accessToken,
            });
        }

        public async Task<T> GetProductByIdAsync<T>(int id, string accessToken)
        {
            return await SendAsync<T>(new ApiRequest()
            {
                ApiType = StaticDetails.APIType.GET,
                Data = id,
                Url = StaticDetails.ProductAPIBase + "/api/ProductAPI/" + id,
                AccessToken = accessToken,
            });
        }

        public async Task<T> UpdateProductAsync<T>(ProductDto productDto, string accessToken)
        {
            return await SendAsync<T>(new ApiRequest()
            {
                ApiType = StaticDetails.APIType.PUT,
                Data = productDto,
                Url = StaticDetails.ProductAPIBase + "/api/ProductAPI",
                AccessToken = accessToken,
            });
        }
    }
}
