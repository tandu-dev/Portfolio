using BlogWebAPIwithJWT.Data;
using Microsoft.EntityFrameworkCore;

namespace BlogWebAPIwithJWT.Services {
    public class PostService : IPostService
    {
        public PostService(BlogDb blogDb)
        {
            db = blogDb;
        }

        public BlogDb db { get; }

        public async Task<EditPost> GetPostAsync(int id)
        {
            return await db.Posts
                .Join(db.Categories, p => p.CategoryId, c=>c.categoryId, (p, c) => new {p, c})
                .Select(s => new EditPost() {Id = s.p.Id, Title = s.p.Title, 
                                        CategoryId = s.p.CategoryId, CategoryName = s.c.CategoryName,
                                        Contents = s.p.Contents, Timestamp = s.p.Timestamp})
                .FirstOrDefaultAsync(s => s.Id == id);                                   
        }

        public async Task<List<EditPost>> GetPostsAsync(PageParameters pg)
        {
            var postCount = db.Posts.Count();
            var numberOfPages = (double)postCount/pg.PageSize;
            var maxPage = (int) Math.Ceiling(numberOfPages);
            if (pg.PageNumber > maxPage)
            {
                pg.PageNumber = maxPage;
            }
            
            return await db.Posts
            .Join(db.Categories, p => p.CategoryId, c=>c.categoryId, (p, c) => new {p, c})
            .OrderByDescending(s => s.p.Id)
            .Skip((pg.PageNumber -1)*pg.PageSize)
            .Take(pg.PageSize)
            .Select(s => new EditPost() {Id = s.p.Id, Title = s.p.Title, 
                                        CategoryId = s.p.CategoryId, CategoryName = s.c.CategoryName,
                                        Contents = s.p.Contents, Timestamp = s.p.Timestamp})
            .ToListAsync();
        }

        public async Task<List<EditPost>> GetPostsAsync()
        {
            return await db.Posts
            .Join(db.Categories, p => p.CategoryId, c=>c.categoryId, (p, c) => new {p, c})
            .Select(s => new EditPost() {Id = s.p.Id, Title = s.p.Title, 
                                        CategoryId = s.p.CategoryId, CategoryName = s.c.CategoryName,
                                        Contents = s.p.Contents, Timestamp = s.p.Timestamp})
            .ToListAsync();
        }
    }
}