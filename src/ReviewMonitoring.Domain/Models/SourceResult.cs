using ReviewMonitoring.Domain.Enums;
using System.Text.Json;

namespace ReviewMonitoring.Domain.Models;

/// <summary>
/// Результат обработки и анализа площадки (Ozon, Wildberrides, итд)
/// </summary>
public class SourceResult
{
    /// <summary>
    /// Имя площадки
    /// </summary>
    public required string Name { get; set; }

    /// <summary>
    /// Ссылка на площадку
    /// </summary>
    public required string Url { get; set; } // ссылка на саму площадку? а надо?

    /// <summary>
    /// Всего отзывов собрано
    /// </summary>
    public int TotalReviews { get; set; }

    /// <summary>
    /// Среднее отзывов за площадку
    /// </summary>
    public decimal AverageRating { get; set; }

    /// <summary>
    /// Результат обработки площадки
    /// </summary>
    public SourceStatus SourceStatus { get; set; }

    /// <summary>
    /// Распределение рейтинга
    /// </summary>
    public Dictionary<int, int> RatingDistribution { get; set; } = []; // для количества оценок

    /// <summary>
    /// По сути список товаров, с которых собирались отзывы, но так как товар зависит от продовца, назван продовцы
    /// </summary>
    public List<ListingResult> Sellers { get; set; } = [];

    /// <summary>
    /// Результат прогона через LLM
    /// </summary>
    public AnalysisSummary? Analysis { get; set; } // пока никогда не будет исопльзоваться, нема логики анализа конкретной площадки через ллм может на 2 или 3 итерации добавлю, а может и шут с ней
    
    /// <summary>
    /// Доп. метаданные
    /// </summary>
    public Dictionary<string, JsonElement>? Extras { get; set; } // для данных анализа конкретных площадок
}
