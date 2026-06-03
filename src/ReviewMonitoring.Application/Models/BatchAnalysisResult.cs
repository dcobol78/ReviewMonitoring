using ReviewMonitoring.Domain.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ReviewMonitoring.Application.Models;
public class BatchAnalysisResult
{
    public Dictionary<string, AnalysisSummary> PerListing { get; set; } = [];
    public AnalysisSummary Overall { get; set; } = new();

    public string? BestSellerUrl { get; set; }
}
