namespace ReviewMonitoring.Domain.Models;

/// <summary>
/// Данные товара агрегированные со всех площадок
/// </summary>
public class ProductInfo
{
    /// <summary>
    /// Словарь названий
    /// Ключ: Ссылка на товар
    /// Значение: Название товара
    /// </summary>
    public Dictionary<string, string> Titles { get; set; } = [];           // url листинга, название

    /// <summary>
    /// Словарь названий
    /// Ключ: Ссылка на товар
    /// Значение: Список ссылко на изображения
    /// </summary>
    public Dictionary<string, List<string>> ImageUrls { get; set; } = [];  // url листинга,  ссылк ина фото

    /// <summary>
    /// Просто список ссылок на все товары, которые были обработаны
    /// </summary>
    public List<string> ProductUrls { get; set; } = [];
}
