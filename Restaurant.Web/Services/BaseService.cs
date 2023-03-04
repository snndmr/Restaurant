using Newtonsoft.Json;
using Restaurant.Web.Models;
using Restaurant.Web.Services.IServices;
using System.Text;

namespace Restaurant.Web.Services
{
    public class BaseService : IBaseService
    {
        public IHttpClientFactory httpClient { get; set; }
        public ResponseDto responseDto { get => throw new NotImplementedException(); set => throw new NotImplementedException(); }

        public BaseService(IHttpClientFactory httpClientFactory)
        {
            httpClient = httpClientFactory;
            responseDto = new ResponseDto();
        }

        public async Task<T> SendAsync<T>(ApiRequest apiRequest)
        {
            string apiContent;

            try
            {
                HttpClient client = httpClient.CreateClient("RestaurantAPI");

                HttpRequestMessage message = new HttpRequestMessage();
                message.Headers.Add("Accept", "application/json");
                message.RequestUri = new Uri(apiRequest.Url ?? string.Empty);

                client.DefaultRequestHeaders.Clear();

                if (apiRequest.Data != null)
                {
                    message.Content = new StringContent(JsonConvert.SerializeObject(apiRequest.Data), Encoding.UTF8, "application/json");
                }

                message.Method = apiRequest.ApiType switch
                {
                    StaticDetails.APIType.POST => HttpMethod.Post,
                    StaticDetails.APIType.PUT => HttpMethod.Put,
                    StaticDetails.APIType.DELETE => HttpMethod.Delete,
                    _ => HttpMethod.Get,
                };

                HttpResponseMessage response = await client.SendAsync(message);
                apiContent = await response.Content.ReadAsStringAsync();
            }
            catch (Exception ex)
            {
                ResponseDto response = new()
                {
                    DisplayMessage = "Error",
                    ErrorMessages = new List<string> { Convert.ToString(ex.Message) },
                    IsSuccess = false,
                };

                apiContent = JsonConvert.SerializeObject(response);
            }

            return JsonConvert.DeserializeObject<T>(apiContent);
        }

        public void Dispose() => GC.SuppressFinalize(true);
    }
}
