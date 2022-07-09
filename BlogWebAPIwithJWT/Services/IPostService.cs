using BlogWebAPIwithJWT.Data;
public interface IPostService {
    public Task<List<EditPost>> GetPostsAsync(PageParameters pg);

    public Task<List<EditPost>> GetPostsAsync();

    public Task<EditPost> GetPostAsync(int id);
}