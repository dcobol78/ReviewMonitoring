using System.Text.Json;

namespace ReviewMonitoring.Domain.Models;

/// <summary>
/// Товар относительно продовца
/// </summary>
public class ListingResult
{
    /// <summary>
    /// Ссылка на товар
    /// </summary>
    public required string Url { get; set; }
    
    /// <summary>
    /// Имя продовца
    /// </summary>
    public required string SellerName { get; set; }

    /// <summary>
    /// Имя товара
    /// </summary>
    public required string ProductTitle { get; set; }

    /// <summary>
    /// Цена
    /// </summary>
    public decimal Price { get; set; }

    /// <summary>
    /// Рейтинг товара
    /// </summary>
    public decimal Rating { get; set; }

    /// <summary>
    /// Рейтинг продовца
    /// </summary>
    public string? SellerRating { get; set; }

    /// <summary>
    /// Количество отзывов
    /// </summary>
    public int ReviewCount { get; set; }

    /// <summary>
    /// Распределение отзывов
    /// </summary>
    public Dictionary<int, int> Distribution { get; set; } = [];

    /// <summary>
    /// Изображения товара
    /// </summary>
    public List<string> ImageUrls { get; set; } = [];

    // на 13.05 отзывы проганяются все пачкой, так что тут что-то будет только если LLM сама решит выделить конкретного продовца
    /// <summary>
    /// Флаги, добавляются после прогона через LLM, 
    /// </summary>
    public List<string> Flags { get; set; } = [];

    // на 13.05 отзывы проганяются все пачкой, так что тут что-то будет только если LLM сама решит выделить конкретного продовца
    /// <summary>
    /// Анализ товара через LLM
    /// </summary>
    public AnalysisSummary? Analysis { get; set; } // пока не знаю, потенциальный оверкил каждый товар в llm гонять, хотя находить потенциальных скамеров вполне себе фитча, на будущее 
    
    /// <summary>
    /// Доп. метаданные
    /// </summary>
    public Dictionary<string, JsonElement>? Extras { get; set; }
}
