using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ReviewMonitoring.Application.Models;
public class IngestionRequest
{
    public required string Query { get; set; }
}
