namespace EfCoreBP.ApiService.DTOs;

public record BookAuthorDetailsDto
{
    public int AuthorId { get; set; }
    public string AuthorName { get; set; } = string.Empty;
    public bool IsPrimaryAuthor { get; set; }
    public DateTime ContributionDate { get; set; }
}