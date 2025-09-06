namespace EfCoreBP.ApiService.DTOs;

public record BookDto
{
    public int Id { get; set; }
    public string Title { get; set; } = string.Empty;
    public string Isbn { get; set; }    = string.Empty;
    public DateTime PublishedDate { get; set; }
    public int PageCount { get; set; }
    public decimal Price { get; set; }
    public string Description { get; set; } = string.Empty;
    public int GenreId { get; set; }
    public string? Genre { get; set; }
    public List<AuthorDto> Authors { get; set; } = [];
}
