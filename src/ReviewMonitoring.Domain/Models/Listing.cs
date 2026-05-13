using System.Text.Json;

namespace ReviewMonitoring.Domain.Models;

/// <summary>
/// Набор отзывов для передачи от парсеров к аналитическим обработчикам
/// </summary>
public class Listing
{
    /// <summary>
    /// Ссылка на товар
    /// </summary>
    public required string Url { get; set; }           // ссылка на конкретного продавца

    /// <summary>
    /// Имя продавца
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
    /// Рейтинг
    /// </summary>
    public decimal Rating { get; set; }

    /// <summary>
    /// Количество отзывов
    /// </summary>
    public int ReviewCount { get; set; }

    /// <summary>
    /// Ссылки на изображения
    /// </summary>
    public List<string> ImageUrls { get; set; } = [];

    /// <summary>
    /// Отзывы
    /// </summary>
    public List<Review> Reviews { get; set; } = [];

    /// <summary>
    /// Доп. метаданные
    /// </summary>
    public Dictionary<string, JsonElement>? Extras { get; set; } // специфика площадки
}