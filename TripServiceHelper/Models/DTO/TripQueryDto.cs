namespace TripServiceHelper.Models.DTOs;

public class TripQueryDto
{
    public string? Keyword { get; set; }
    public string? Status { get; set; }
    public string? TripType { get; set; }
    public int? CityId { get; set; }
    public DateTime? StartFrom { get; set; }
    public DateTime? StartTo { get; set; }
    public int? MinCapacity { get; set; }
    public int? MaxCapacity { get; set; }
    public string? SortBy { get; set; }
    public int Page { get; set; } = 1;
    public int PageSize { get; set; } = 10;
}