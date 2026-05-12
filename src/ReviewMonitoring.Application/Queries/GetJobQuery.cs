using MediatR;
using ReviewMonitoring.Domain.Domain;

namespace ReviewMonitoring.Application.Queries;
public record GetJobQuery(Guid JobId) : IRequest<Job?>;