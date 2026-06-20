using ReviewMonitoring.Domain.Enums;
using ReviewMonitoring.Domain.Models;
using System.Text.Json.Serialization;

namespace ReviewMonitoring.Domain.Domain;

/// <summary>
/// Задача обработки
/// </summary>
public class Job
{
    [JsonConstructor]
    private Job() { }

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
    /// Конфигурация поиска
    /// </summary>
    public IngestionConfig SearchConfig { get; set; }

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
    /// Сколько товаров собрано
    /// Обновляется каждый обработанный источник
    /// </summary>
    public int ListingsCollected { get; set; }

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


    public static Job Create(string query, 
        ProcessingMode mode, 
        IReadOnlyList<string> providers,
        IngestionConfig? searchConfig = null)
    {
        return new Job
        {
            Id = Guid.NewGuid(),
            Query = query,
            Mode = mode,
            Status = JobStatus.Pending,
            CreatedAt = DateTime.UtcNow,
            SearchConfig = searchConfig ?? new IngestionConfig(),
            SourceStatuses = providers.ToDictionary(
                p => p,
                p => SourceStatus.Pending)
        };
    }

    public static Job Restore(
        Guid id,
        string query,
        ProcessingMode mode,
        JobStatus status,
        DateTime createdAt,
        IngestionConfig searchConfig,
        DateTime? completedAt,
        string? errorMessage,
        ProcessingResult? result,
        int reviewsCollected = 0,
        int listingsCollected = 0,
        Dictionary<string, SourceStatus>? sourceStatuses = null)
    {
        return new Job
        {
            Id = id,
            Query = query,
            Mode = mode,
            Status = status,
            CreatedAt = createdAt,
            SearchConfig = searchConfig,
            CompletedAt = completedAt,
            ErrorMessage = errorMessage,
            Result = result,
            ReviewsCollected = reviewsCollected,
            ListingsCollected = listingsCollected,
            SourceStatuses = sourceStatuses ?? [],
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

    public void AddSourceResult(SourceResult result)
    {
        if (result is null)
            return;

        Result ??= new ProcessingResult
        {
            Product = new ProductInfo(),
        };

        // Добавляем результат площадки
        Result.Sources.Add(result);

        // Обновляем ProductInfo из листингов площадки
        foreach (var seller in result.Listings)
        {
            Result.Product.ProductUrls.Add(seller.Url);

            if (!string.IsNullOrEmpty(seller.ProductTitle))
                Result.Product.Titles[seller.Url] = seller.ProductTitle;

            if (seller.ImageUrls.Count > 0)
                Result.Product.ImageUrls[seller.Url] = seller.ImageUrls;
        }

        // Обновляем статус источника
        SourceStatuses[result.Name] = SourceStatus.Completed;
    }

    //TODO: Сделать
    public void SetFinalProcessingResult(ProcessingResult result)
    {
        if (result is null)
            return;

        // Сохраняем накопленный ProductInfo
        result.Product = Result?.Product ?? new ProductInfo();

        Result = result;
    }

    public void AddCollectedReviews(int count)
    {
        ReviewsCollected += count;
    }

    public void AddCollectedListings(int count)
    {
        ListingsCollected += count;
    }

    public void Ping()
    {
        LastPingAt = DateTime.UtcNow;
    }
}
