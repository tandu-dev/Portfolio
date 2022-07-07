using BlazorBlog.Data.Model;
namespace BlazorBlog.Data.Services{
    public interface IApiService
    {
        Task<IEnumerable<Post>> GetPosts(PageParameters pg);

        Task<Post> GetPostAsync(string Id);

        Task<TokenWrapper> Login(BlogCredential blogCredential);
    }
}