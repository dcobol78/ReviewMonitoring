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
public static class ConstsPostgresDebug
{
    /// <summary>
    /// Строка соединения PostgresSQL
    /// </summary>
    public const string DebugPostgresConnection = "Host=localhost;Port=5432;Database=reviewmonitoring;Username=postgres;Password=yourpassword";
}

#endif
