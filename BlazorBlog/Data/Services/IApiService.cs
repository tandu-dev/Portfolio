using BlazorBlog.Data.Model;
namespace BlazorBlog.Data.Services{
    public interface IApiService
    {
        Task<IEnumerable<Post>> GetPosts(PageParameters pg);
    }
}