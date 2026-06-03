namespace ReviewMonitoring.Ingestion.Interfaces;
public interface IHttpIngestionClient
{
    Task<string?> GetAsync(string url, 
        CancellationToken ct,
        Dictionary<string, string>? headers = null);

    Task<T?> GetJsonAsync<T>(string url, 
        CancellationToken ct,
        Dictionary<string, string>? headers = null);


}