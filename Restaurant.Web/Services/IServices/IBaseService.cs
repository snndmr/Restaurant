using Restaurant.Web.Models;

namespace Restaurant.Web.Services.IServices
{
    public interface IBaseService : IDisposable
    {
        ResponseDto responseDto { get; set; }

        Task<T> SendAsync<T>(ApiRequest apiRequest);
    }
}
