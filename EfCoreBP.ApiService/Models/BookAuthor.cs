using System.Text.Json.Serialization;

namespace EfCoreBP.ApiService.Models;

// Junction table for many-to-many relationship between Book and Author
public class BookAuthor
{
    public int BookId { get; set; }
    public int AuthorId { get; set; }
    public bool IsPrimaryAuthor { get; set; }
    public DateTime ContributionDate { get; set; }

    // Navigation properties
    [JsonIgnore]
    public virtual Book Book { get; set; } = null!;
    [JsonIgnore]
    public virtual Author Author { get; set; } = null!;
}