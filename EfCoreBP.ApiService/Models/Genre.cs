using System.Text.Json.Serialization;

namespace EfCoreBP.ApiService.Models;

public class Genre
{
    public int Id { get; set; }
    public bool IsActive { get; set; } = true;
    public string Name { get; set; } = string.Empty;
    public string Description { get; set; } = string.Empty;

    // Navigation properties
    [JsonIgnore]
    public virtual ICollection<Book> Books { get; set; } = new List<Book>();
}