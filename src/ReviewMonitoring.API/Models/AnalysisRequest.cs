using ReviewMonitoring.Core.Enums;

namespace ReviewMonitoring.API.Models
{
    public class AnalysisRequest
    {
        public string Query { get; set; }
        public AnalysisMode Mode { get; set; } = AnalysisMode.Full; // дефолт
    }
}
