using MediatR;
using ReviewMonitoring.Domain.Enums;

namespace ReviewMonitoring.Application.Commands;

public record PingJobCommand(Guid jobId) : IRequest<bool>;