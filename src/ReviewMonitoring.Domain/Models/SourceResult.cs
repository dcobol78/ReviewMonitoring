using System.Text.Json;

namespace ReviewMonitoring.Domain.Models;

public class SourceResult
{
    public required string Name { get; set; }
    public required string Url { get; set; }
    public int TotalReviews { get; set; }
    public decimal AverageRating { get; set; }
    public Dictionary<int, int> Distribution { get; set; } = [];
    public List<SellerResult> Sellers { get; set; } = [];
    public Dictionary<string, JsonElement>? Extras { get; set; } // Для данных анализа конкретных площадок
}
