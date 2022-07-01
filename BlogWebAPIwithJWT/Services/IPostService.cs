using BlogWebAPIwithJWT.Data;
public interface IPostService {
    public Task<List<Post>> GetPostsAsync(PageParameters pg);

    public Task<List<Post>> GetPostsAsync();

    public Task<Post> GetPostAsync(int id);
}