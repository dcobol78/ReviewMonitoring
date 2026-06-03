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
    public int TotalListingsProcessed { get; set; }
    public string? ErrorMessage { get; set; }

    public static IngestionResult Ok(int totalListings = 0) => new()
    {
        Success = true,
        TotalListingsProcessed = totalListings
    };

    public static IngestionResult Fail(string errorMessage = "") => new()
    {
        Success = false,
        ErrorMessage = errorMessage
    };
}
