using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ReviewMonitoring.Infrastructure.Postgres.Consts;

#if DEBUG

/// <summary>
/// Константы для дебага
/// </summary>
public static class ConstsRedisDebug
{
    /// <summary>
    /// Строка соединения Redis
    /// </summary>
    public const string DebugRedisConnection = "localhost:6379";
}

#endif
