namespace ReviewMonitoring.Domain.Models;

public class Review
{
    public required string Id { get; set; }
    public required string Text { get; set; }
    public int Rating { get; set; }
    public DateTime Date { get; set; }
    public string? AuthorName { get; set; }
    public int Likes { get; set; }
    public string? SellerName { get; set; }
    public string Source { get; set; } = string.Empty;
}
