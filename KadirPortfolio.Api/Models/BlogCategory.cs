using System.Text.Json.Serialization;

namespace KadirPortfolio.Api.Models
{
    public class BlogCategory
    {
        public int Id { get; set; }
        public string Name { get; set; } = string.Empty;
        public string? NameEn { get; set; }
        public string? Icon { get; set; }
        
        [JsonIgnore]
        public List<BlogPost> BlogPosts { get; set; } = new();
    }
}
