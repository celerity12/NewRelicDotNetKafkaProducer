using System.Threading.Tasks;
using System.Net.Http;
using System.Threading.Tasks;

namespace HellowWebAppSR.Services
{
    public interface IBingService
    {
        Task<string> GetBingSnippetAsync(int maxLength = 5000);
        Task<HttpResponseMessage> A();
        Task<string> B(HttpResponseMessage response, int maxLength);
    }
}


namespace HellowWebAppSR.Services
{
    public class BingService : IBingService
    {
        private readonly HttpClient _httpClient;

        public BingService(HttpClient httpClient)
        {
            _httpClient = httpClient;
        }

        public async Task<string> GetBingSnippetAsync(int maxLength = 5000)
        {
            var response = await A();
            return await B(response, maxLength);
        }

        public async Task<HttpResponseMessage> A()
        {
            // calls C() to get the response from Bing  
            HttpResponseMessage response = await C();
            return response;
        }

        public async Task<HttpResponseMessage> C()
        {
            HttpResponseMessage response = await _httpClient.GetAsync("https://www.bing.com");
            response.EnsureSuccessStatusCode();
            return response;
        }

        public async Task<string> B(HttpResponseMessage response, int maxLength)
        {
            string html = await response.Content.ReadAsStringAsync();
            return html.Substring(0, System.Math.Min(maxLength, html.Length));
        }
    }
}
