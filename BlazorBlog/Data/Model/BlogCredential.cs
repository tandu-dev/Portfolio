using System.ComponentModel.DataAnnotations;

namespace BlazorBlog.Data.Model
{
    public class BlogCredential{
        [Required]
        public string UserName {get; set;}

        [Required]
        public string Password {get; set;}
    }
    public class TokenWrapper {
           public string token { get; set; }
    }
}