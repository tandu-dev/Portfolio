  namespace BlogWebAPIwithJWT.Data{
    public class Post
    {
        public int Id { get; set; }
        public string? Title { get; set; }
        public string? Contents { get; set; }
        public DateTime Timestamp {get; set;}
        public int CategoryId {get; set;}

    }
    public class EditPost : Post
    {        
      public string? CategoryName {get; set;}
    }
  }