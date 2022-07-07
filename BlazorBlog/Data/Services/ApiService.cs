using BlazorBlog.Data.Model;
using System.Text.Json;

namespace BlazorBlog.Data.Services {
    public class ApiService : IApiService
    {
        public ApiService(IHttpClientFactory clientFactory, IConfiguration config)
        {
            ClientFactory = clientFactory;
            Config = config;
        }

        public IHttpClientFactory ClientFactory { get; }
        public IConfiguration Config { get; }

        public async Task<Post> GetPostAsync(string Id)
        {
            var client = ClientFactory.CreateClient();
            var url = $"{Config["GetPostAddress"]}/{Id}";
            var response = await client.GetAsync(url);
            if(response.IsSuccessStatusCode)
            {
                using var responseStream =  await response.Content.ReadAsStreamAsync();
                var jsonOptions = new JsonSerializerOptions() {
                    PropertyNamingPolicy = JsonNamingPolicy.CamelCase
                };
                var jsonString = await response.Content.ReadAsStringAsync();
                var postResults = await JsonSerializer.DeserializeAsync<Post>(
                   responseStream, jsonOptions
                );
                return postResults;
            }
            else{
                throw new Exception(response.StatusCode.ToString());
            }
        }

        public async Task<IEnumerable<Post>> GetPosts(PageParameters pg)
        {
            IEnumerable<Post> posts = Array.Empty<Post>();
            var client = ClientFactory.CreateClient();
            var url = $"{Config["GetPostAddress"]}/{pg.PageNumber}/{pg.PageSize}";
            var response = await client.GetAsync(url);
            if(response.IsSuccessStatusCode)
            {
                using var responseStream =  await response.Content.ReadAsStreamAsync();
                var jsonOptions = new JsonSerializerOptions() {
                    PropertyNamingPolicy = JsonNamingPolicy.CamelCase
                };
                var jsonString = await response.Content.ReadAsStringAsync();
                var postResults = await JsonSerializer.DeserializeAsync<IEnumerable<Post>>(
                   responseStream, jsonOptions
                );
                return postResults;
            }
            else{
                throw new Exception(response.StatusCode.ToString());
            }
            
        }

        public async Task<TokenWrapper> Login(BlogCredential blogCredential)
        {
           var client = ClientFactory.CreateClient();
            var url = $"{Config["GetLoginAddress"]}";
            var response = await client.PostAsJsonAsync<BlogCredential>(url, blogCredential);
            if(response.IsSuccessStatusCode)
            {
                using var responseStream =  await response.Content.ReadAsStreamAsync();
                var jsonOptions = new JsonSerializerOptions() {
                    PropertyNamingPolicy = JsonNamingPolicy.CamelCase
                };
                var jsonString = await response.Content.ReadAsStringAsync();
                var Result = await JsonSerializer.DeserializeAsync<TokenWrapper>(
                   responseStream, jsonOptions
                );
                return Result;
            }
            else{
                throw new Exception(response.StatusCode.ToString());
            }
        }
    }
}