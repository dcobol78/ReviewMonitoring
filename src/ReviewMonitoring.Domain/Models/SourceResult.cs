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
    public required string Url { get; set; }

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
    /// Распределение рейтинга по всем найденным товарам на площадке
    /// </summary>
    public Dictionary<int, int> SourceRatingDistribution { get; set; } = [];

    /// <summary>
    /// По сути список товаров, с которых собирались отзывы, но так как товар зависит от продовца, назван продовцы
    /// </summary>
    public List<Listing> Listings { get; set; } = [];

    /// <summary>
    /// Доп. метаданные
    /// </summary>
    public Dictionary<string, JsonElement>? Extras { get; set; } // для данных анализа конкретных площадок
}
