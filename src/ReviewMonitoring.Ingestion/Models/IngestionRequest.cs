using ReviewMonitoring.Domain.Models;

namespace ReviewMonitoring.Ingestion.Models;
public class IngestionRequest
{
    public required List<string> Urls { get; set; }
    public IngestionConfig Config { get; set; } = new();
}
