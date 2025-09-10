namespace EfCoreBP.ApiService.DTOs;

public record BookWithDetailsDto
{
    public int Id { get; set; }
    public string Title { get; set; } = string.Empty;
    public DateTime PublishedDate { get; set; }
    public int PageCount { get; set; }
    public decimal Price { get; set; }
    public string Description { get; set; } = string.Empty;
    public int GenreId { get; set; }
    public string? Genre { get; set; }
    public IEnumerable<BookAuthorDetailsDto> Authors { get; set; } = [];
}