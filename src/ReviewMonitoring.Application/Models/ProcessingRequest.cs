using ReviewMonitoring.Domain.Enums;
using ReviewMonitoring.Domain.Models;

namespace ReviewMonitoring.Application.Models;

public class ProcessingRequest
{
    public IReadOnlyCollection<Review> Reviews { get; set; }
    public ProcessingMode Mode { get; set; }
}
