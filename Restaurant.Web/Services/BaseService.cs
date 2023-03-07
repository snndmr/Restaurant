using Newtonsoft.Json;
using Restaurant.Web.Models;
using Restaurant.Web.Services.IServices;
using System.Net.Http.Headers;
using System.Text;

namespace Restaurant.Web.Services
{
    public class BaseService : IBaseService
    {
        private IHttpClientFactory HttpClient { get; set; }
        public ResponseDto ResponseDto { get; set; }

        public BaseService(IHttpClientFactory httpClientFactory)
        {
            HttpClient = httpClientFactory;
            ResponseDto = new ResponseDto();
        }

        public async Task<T> SendAsync<T>(ApiRequest apiRequest)
        {
            string apiContent;

            try
            {
                HttpClient client = HttpClient.CreateClient("RestaurantAPI");

                HttpRequestMessage message = new HttpRequestMessage();
                message.Headers.Add("Accept", "application/json");
                message.RequestUri = new Uri(apiRequest.Url ?? string.Empty);

                client.DefaultRequestHeaders.Clear();

                if (apiRequest.Data != null)
                {
                    message.Content = new StringContent(JsonConvert.SerializeObject(apiRequest.Data), Encoding.UTF8, "application/json");
                }

                if (!string.IsNullOrEmpty(apiRequest.AccessToken))
                {
                    client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", apiRequest.AccessToken);
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
