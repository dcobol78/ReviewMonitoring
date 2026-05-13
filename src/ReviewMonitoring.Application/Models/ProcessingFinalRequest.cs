using ReviewMonitoring.Domain.Enums;
using ReviewMonitoring.Domain.Models;

namespace ReviewMonitoring.Application.Models;

public class ProcessingFinalRequest
{
    public required IReadOnlyCollection<Review> Reviews { get; set; }
    public ProcessingResult? ProcessingResult { get; set; }
    public ProcessingMode Mode { get; set; }
}
