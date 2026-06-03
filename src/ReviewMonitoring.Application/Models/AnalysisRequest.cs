using ReviewMonitoring.Domain.Enums;
using ReviewMonitoring.Domain.Models;

namespace ReviewMonitoring.Application.Models
{
    public class AnalysisRequest
    {
        public required string Query { get; set; }
        public ProcessingMode Mode { get; set; }
        public IngestionConfig? Config { get; set; }
    }
}
