using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ReviewMonitoring.Core.Enums;
public enum JobStatus
{
    Pending,
    Parsing,
    Parsed,
    Analyzing,
    Completed,
    PartiallyCompleted,
    Failed,
    Cancelled,
    Paused        // Пока бесполезно, да и вообще наверное не надо, перезапустить анализ будет логичнее, не? Пусть пока будет.
}
