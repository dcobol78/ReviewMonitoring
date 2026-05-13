using ReviewMonitoring.Domain.Enums;
using ReviewMonitoring.Domain.Models;

namespace ReviewMonitoring.Domain.Domain;

/// <summary>
/// Задача обработки
/// </summary>
public class Job
{
    //Можно и в int перегнать впринципе
    /// <summary>
    /// ID работы
    /// </summary>
    public required Guid Id { get; set; }

    // На данном этапе принимаются только ссылки
    /// <summary>
    /// Запрос, ссылка или название товара
    /// </summary>
    public required string Query { get; set; }

    /// <summary>
    /// Режим анализа
    /// </summary>
    public required ProcessingMode Mode { get; set; }

    /// <summary>
    /// Статус работы
    /// </summary>
    public JobStatus Status { get; set; } = JobStatus.Pending;

    //TODO: по сути дублирует данные глубже, если прям надо можно возврщаать собирая данные из Result.
    public Dictionary<string, SourceStatus> SourceStatuses { get; set; } = [];

    /// <summary>
    /// Сколько отзывов собрано на данный момент
    /// Обновляется каждый обработанный источник
    /// </summary>
    public int ReviewsCollected { get; set; }

    /// <summary>
    /// Результат обработки
    /// </summary>
    public ProcessingResult? Result { get; set; }

    /// <summary>
    /// Ошибка обработки если есть
    /// </summary>
    public string? ErrorMessage { get; set; }

    /// <summary>
    /// Когда создана задача
    /// </summary>
    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;

    /// <summary>
    /// Когда завершена задача
    /// </summary>
    public DateTime? CompletedAt { get; set; }

    /// <summary>
    /// Когда последний Ping
    /// </summary>
    public DateTime? LastPingAt { get; set; }


    public static Job Create(string query, ProcessingMode mode, IReadOnlyList<string> providers)
    {
        return new Job
        {
            Id = Guid.NewGuid(),
            Query = query,
            Mode = mode,
            Status = JobStatus.Pending,
            CreatedAt = DateTime.UtcNow,
            SourceStatuses = providers.ToDictionary(
                p => p,
                p => SourceStatus.Pending)
        };
    }

    public void Cancel()
    {
        Status = JobStatus.Cancelled;
        CompletedAt = DateTime.UtcNow;
    }

    public void Complete(ProcessingResult result)
    {
        Result = result;
        Status = JobStatus.Completed;
        CompletedAt = DateTime.UtcNow;
    }

    public void Fail(string errorMessage)
    {
        ErrorMessage = errorMessage;
        Status = JobStatus.Failed;
        CompletedAt = DateTime.UtcNow;
    }

    public void StartParsing()
    {
        Status = JobStatus.Parsing;
    }

    public void StartAnalyzing()
    {
        Status = JobStatus.Analyzing;
    }

    //TODO: Доделать
    public void AddSourceResult(SourceResult result)
    {

        if (result == null)
            return;

        Result ??= new ProcessingResult
        {
            Product = new ProductInfo(),
            Aggregate = new AggregateStats()
        };

        Result.Product.ProductUrls.Add(result.Url);
    }

    //TODO: Сделать
    public void SetFinalProcessingResult(ProcessingResult result)
    {

        if (result == null)
            return;
    }

    public void AddCollectedReviews(int count)
    {
        ReviewsCollected += count;
    }

    public void Ping()
    {
        LastPingAt = DateTime.UtcNow;
    }
}
