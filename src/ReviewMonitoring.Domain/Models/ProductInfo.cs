namespace ReviewMonitoring.Domain.Models;

public class ProductInfo
{
    public required string Title { get; set; }
    public List<string> ImageUrls { get; set; } = [];
}
