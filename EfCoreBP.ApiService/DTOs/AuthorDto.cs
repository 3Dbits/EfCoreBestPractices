namespace EfCoreBP.ApiService.DTOs;

public record AuthorDto
{
    public int Id { get; set; }
    public string Name { get; set; }
    public bool IsPrimaryAuthor { get; set; }
}