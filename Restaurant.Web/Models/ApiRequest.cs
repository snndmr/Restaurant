using static Restaurant.Web.StaticDetails;

namespace Restaurant.Web.Models
{
    public class ApiRequest
    {
        public APIType ApiType { get; set; }
        public string? Url { get; set; }
        public object? Data { get; set; }
        public string? AccessToken { get; set; }
    }
}
