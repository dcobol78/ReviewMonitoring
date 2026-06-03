using ReviewMonitoring.Application.Models;
using ReviewMonitoring.Ingestion.Models;

//TODO: не хвататет качественного поиска в других платформах, если клиент дает ссылку на OZON, как мы найдем тот же товар в другой среде?
//В идеале отдать эту задачу тоже LLM, аналитически это будет не просто разрешить хорошо, разве что по точному совпадению искать.
//22.05: делаю поиск, поиск будет с помощью LLM, задача LLM выделить корень товара и выбрать подходящие из списка найденных

namespace ReviewMonitoring.Ingestion.Interfaces;
public interface IIngestionProvider
{
    string ProviderName { get; }

    bool CanHandle(string? url);

    Task<string?> FetchTitleAsync(string url, CancellationToken ct);

    Task<IngestionProgress> IngestAsync(IngestionRequest request, CancellationToken ct);

    Task<List<ProductCandidate>> SearchAsync(ProductSearchTerms terms, CancellationToken ct);
}
