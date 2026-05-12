
using ReviewMonitoring.Domain.Enums;

namespace ReviewMonitoring.API.Models.v1
{
    public class AnalysisRequest
    {
        public string Query { get; set; }
        public ProcessingMode Mode { get; set; } = ProcessingMode.LLM; // дефолт
    }
}
