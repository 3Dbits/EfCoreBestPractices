using System.Text.Json.Serialization;

namespace EfCoreBP.ApiService.Models;

public class Author
{
    public int Id { get; set; }
    public string FirstName { get; set; } = string.Empty;
    public string LastName { get; set; } = string.Empty;
    public string Email { get; set; } = string.Empty;
    public DateTime DateOfBirth { get; set; }
    public string Biography { get; set; } = string.Empty;
    public ulong RowVersion { get; set; }

    // Navigation properties
    [JsonIgnore]
    public virtual ICollection<BookAuthor> BookAuthors { get; set; } = new List<BookAuthor>();
}