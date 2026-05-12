using ReviewMonitoring.Domain.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ReviewMonitoring.Application.Models;
public class IngestionResult
{
    public bool Success { get; set; }
    public List<Review> Reviews { get; set; } = [];
    public string? ErrorMessage { get; set; }
}
