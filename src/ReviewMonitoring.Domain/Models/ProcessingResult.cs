namespace ReviewMonitoring.Domain.Models;

/// <summary>
/// Финальный результат обработки
/// </summary>
public class ProcessingResult
{
    /// <summary>
    /// Данные о товаре в целом
    /// </summary>
    public ProductInfo Product { get; set; } = new();

    /// <summary>
    /// Статистические данные товара
    /// </summary>
    public AggregateStats? Aggregate { get; set; }

    /// <summary>
    /// Данные каждого источника
    /// </summary>
    public List<SourceResult> Sources { get; set; } = [];
}

