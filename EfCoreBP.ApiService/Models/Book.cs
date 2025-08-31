using System.Text.Json.Serialization;

namespace EfCoreBP.ApiService.Models;

public class Book
{
    public int Id { get; set; }
    public string Title { get; set; } = string.Empty;
    public string ISBN { get; set; } = string.Empty;
    public DateTime PublishedDate { get; set; }
    public int PageCount { get; set; }
    public decimal Price { get; set; }
    public string Description { get; set; } = string.Empty;
    
    // Foreign keys
    public int GenreId { get; set; }
    
    // Navigation properties
    public virtual Genre Genre { get; set; } = null!;
    [JsonIgnore]
    public virtual ICollection<BookAuthor> BookAuthors { get; set; } = new List<BookAuthor>();
}