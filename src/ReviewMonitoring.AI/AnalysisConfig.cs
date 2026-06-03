using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ReviewMonitoring.AI;
public class AnalysisConfig
{
    public int MaxReviewsPerListing { get; set; } = 40;
    public int MaxTotalReviews { get; set; } = 200;
}