namespace ReviewMonitoring.Ingestion.Ozon.Models;
internal class OzonProductData
{
    public string Sku { get; set; } = string.Empty;
    public string Title { get; set; } = string.Empty;
    public decimal Price { get; set; }
    public decimal Rating { get; set; }
    public int ReviewCount { get; set; }
    public List<string> ImageUrls { get; set; } = [];
    public string SellerName { get; set; } = string.Empty;
    public string SellerUrl { get; set; } = string.Empty;
    public string? OtherSellersModalUrl { get; set; }
    public int OtherSellersCount { get; set; }
}