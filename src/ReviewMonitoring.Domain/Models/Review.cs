//TODO: Добавить ссылки на картинги отзывов, правда они будут свалены в одну кучу на клиенте

namespace ReviewMonitoring.Domain.Models;

/// <summary>
/// Отзыв
/// </summary>
public class Review
{
    // как будто абсолютно бесполезно, он нигде харниться не будет, кроме как в контексте сбора и анализа
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

    // может пригодится, если мы будем определять вес и более новые отзывы будут ценнее, но это уже фантастика
    /// <summary>
    /// Дата
    /// </summary>
    public DateTime Date { get; set; }

    // как будто бы тоже бесполезно
    /// <summary>
    /// Имя автора отзыва
    /// </summary>
    public string? AuthorName { get; set; }

    // тоже бесполезно, без веса
    /// <summary>
    /// Количество лайков на отзыве
    /// </summary>
    public int Likes { get; set; }
}