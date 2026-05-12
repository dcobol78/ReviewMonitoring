using MediatR;
using ReviewMonitoring.Domain.Enums;

namespace ReviewMonitoring.Application.Commands;

public record CreateJobCommand(
    string query,
    ProcessingMode mode,
    bool useSmartMatching = false) : IRequest<Guid>;