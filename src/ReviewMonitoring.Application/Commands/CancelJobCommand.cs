using MediatR;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ReviewMonitoring.Application.Commands;
public record CancelJobCommand(Guid JobId) : IRequest<bool>;
