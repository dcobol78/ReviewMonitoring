namespace ReviewMonitoring.Ingestion.Interfaces;
public interface IBrowserSession : IAsyncDisposable
{
    Task InitializeAsync();
    Task<string> GetPageHtmlAsync(string url, CancellationToken ct);
    Task<string> GetPageHtmlWithScrollAsync(string url, int scrollCount, CancellationToken ct);
}
