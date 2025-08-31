namespace EfCoreBP.ApiService.DTOs;

/// <summary>
/// Data Transfer Object for Book containing only the title
/// Used for efficient data transfer when only book name is needed
/// </summary>
public record BookTitleDto
{
    public string Title { get; set; } = string.Empty;
}