namespace ReviewMonitoring.AI.Client;
public interface IAiClient
{
    Task<string> CompleteAsync(string prompt, CancellationToken ct);
    Task<T?> CompleteStructuredAsync<T>(string prompt, CancellationToken ct);
}