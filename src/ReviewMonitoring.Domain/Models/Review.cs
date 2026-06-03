//TODO: Добавить ссылки на картинги отзывов, правда они будут свалены в одну кучу на клиенте

namespace ReviewMonitoring.Domain.Models;

/// <summary>
/// Отзыв
/// </summary>
public class Review
{
    /// <summary>
    /// ID отзыва
    /// </summary>
    public required string Id { get; set; }

    /// <summary>
    /// Ссылка на товар
    /// </summary>
    public required string ListingUrl { get; set; }

    /// <summary>
    /// Содержание
    /// </summary>
    public required string Text { get; set; }

    /// <summary>
    /// Оценка
    /// </summary>
    public int Rating { get; set; }

    /// <summary>
    /// Дата
    /// </summary>
    public DateTime Date { get; set; }

    /// <summary>
    /// Имя автора отзыва
    /// </summary>
    public string? AuthorName { get; set; }

    /// <summary>
    /// Количество лайков на отзыве
    /// </summary>
    public int Likes { get; set; }
}