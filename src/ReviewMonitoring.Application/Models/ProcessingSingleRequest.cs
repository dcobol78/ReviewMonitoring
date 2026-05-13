using ReviewMonitoring.Domain.Enums;
using ReviewMonitoring.Domain.Models;

namespace ReviewMonitoring.Application.Models;

public class ProcessingSingleRequest
{
    public required IReadOnlyCollection<Review> Reviews { get; set; }
    public required string SourceName { get; set; }
    public ProcessingMode Mode { get; set; }
}
