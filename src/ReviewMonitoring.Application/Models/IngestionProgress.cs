using ReviewMonitoring.Domain.Enums;
using ReviewMonitoring.Domain.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ReviewMonitoring.Application.Models;
public class IngestionProgress
{
    public required string SourceName { get; set; }
    public SourceStatus Status { get; set; }
    public int ReviewsCollected { get; set; }
    public List<Review> Reviews { get; set; } = [];
}
