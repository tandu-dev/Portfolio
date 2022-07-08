using BlazorBlog.Data.Model;
using System.Text.Json;

namespace BlazorBlog.Data.Services {
    
    public interface IApiService
    {
        Task<IEnumerable<Post>> GetPosts(PageParameters pg);

        Task<Post> GetPostAsync(string Id);

    }
    public class ApiService : IApiService
    {
        public ApiService(IHttpService httpService, IConfiguration config)
        {
            HttpService = httpService;
            Config = config;
        }

        public IHttpService HttpService { get; }
        public IConfiguration Config { get; }

        public async Task<Post> GetPostAsync(string Id)
        {            
            var url = $"{Config["GetPostAddress"]}/{Id}";            
            var response = await HttpService.Get<Post>(url);
            return response;
            // if(response.IsSuccessStatusCode)
            // {
            //     using var responseStream =  await response.Content.ReadAsStreamAsync();
            //     var jsonOptions = new JsonSerializerOptions() {
            //         PropertyNamingPolicy = JsonNamingPolicy.CamelCase
            //     };
            //     var postResults = await JsonSerializer.DeserializeAsync<Post>(
            //        responseStream, jsonOptions
            //     );
            //     return postResults;
            // }
            // else{
            //     throw new Exception(response.StatusCode.ToString());
            // }
        }

        public async Task<IEnumerable<Post>> GetPosts(PageParameters pg)
        {
            IEnumerable<Post> posts = Array.Empty<Post>();
           
            var url = $"{Config["GetPostAddress"]}/{pg.PageNumber}/{pg.PageSize}";
            var response = await HttpService.Get<IEnumerable<Post>>(url);
            return response;
            // if(response.IsSuccessStatusCode)
            // {
            //     using var responseStream =  await response.Content.ReadAsStreamAsync();
            //     var jsonOptions = new JsonSerializerOptions() {
            //         PropertyNamingPolicy = JsonNamingPolicy.CamelCase
            //     };
            //     var postResults = await JsonSerializer.DeserializeAsync<IEnumerable<Post>>(
            //        responseStream, jsonOptions
            //     );
            //     return postResults;
            // }
            // else{
            //     throw new Exception(response.StatusCode.ToString());
            // }
            
        }

        
    }
}