using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ReviewMonitoring.Infrastructure.Redis.Consts;

#if DEBUG

/// <summary>
/// Константы для дебага
/// </summary>
internal static class ConstsRedisDebug
{
    /// <summary>
    /// Строка соединения Redis
    /// </summary>
    public const string DebugRedisConnection = "localhost:6379";
}

#endif
