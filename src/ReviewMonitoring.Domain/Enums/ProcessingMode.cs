using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ReviewMonitoring.Domain.Enums
{
    public enum ProcessingMode
    {
        Stats,  // только статистика
        LLM        // статистика + иишка
    }
}
