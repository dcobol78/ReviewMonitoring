namespace ReviewMonitoring.Domain.Models;

/// <summary>
/// Статистические данные
/// </summary>
public class AggregateStats
{
    /// <summary>
    /// Количество источников
    /// </summary>
    public int TotalSources { get; set; }

    /// <summary>
    /// Количество продовцов
    /// </summary>
    public int TotalSellers { get; set; }

    /// <summary>
    /// Количество отзывов
    /// </summary>
    public int TotalReviews { get; set; }

    /// <summary>
    /// Средняя оценка
    /// </summary>
    public decimal AverageRating { get; set; }

    /// <summary>
    /// Минимальная цена
    /// </summary>
    public decimal MinPrice { get; set; }

    /// <summary>
    /// Максимальная цена
    /// </summary>
    public decimal MaxPrice { get; set; }

    /// <summary>
    /// Распределение оценок
    /// </summary>
    public Dictionary<int, int> Distribution { get; set; } = [];

    // По идее иишка должна это решать, но можно и аналитически задолбаться, голодным поганять.
    /// <summary>
    /// Лучший вариант для покупки
    /// </summary>
    public Listing? BestSeller { get; set; }
}