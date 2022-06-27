    namespace BlazorBlog.Data.Model {
    
        public class Post
        {
            public int Id { get; set; }
            public string? Title { get; set; }
            public string? Contents { get; set; }
            public DateTime Timestamp {get; set;}
            public int CategoryId {get; set;}
        }
        public class Category {
            public int categoryId {get; set;}

            public string? CategoryName {get;set;}
        }
    }