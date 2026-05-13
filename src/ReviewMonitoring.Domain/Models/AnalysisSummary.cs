namespace ReviewMonitoring.Domain.Models;

/// <summary>
/// Результат анализа через LLM
/// </summary>
public class AnalysisSummary
{
    /// <summary>
    /// Достоинства
    /// </summary>
    public List<string> Pros { get; set; } = [];

    /// <summary>
    /// Недостатки
    /// </summary>
    public List<string> Cons { get; set; } = [];

    /// <summary>
    /// Ключевые слова
    /// </summary>
    public List<string> Keywords { get; set; } = [];

    /// <summary>
    /// Флаги
    /// </summary>
    public List<string> Flags { get; set; } = [];

    /// <summary>
    /// Вывод
    /// </summary>
    public string Summary { get; set; } = string.Empty;
}
