using Microsoft.Extensions.Logging;

namespace ReviewMonitoring.Shared;

/// <summary>
/// Класс для хранения логгера приложения
/// </summary>
public static class AppLog //приколюха https://stackoverflow.com/questions/48676152/asp-net-core-web-api-logging-from-a-static-class
{
    /// <summary>
    /// Фабрика логера
    /// </summary>
    public static ILoggerFactory LoggerFactory { get; set; }


    /// <summary>
    /// Получить логер
    /// </summary>
    /// <typeparam name="T"></typeparam>
    /// <returns></returns>
    public static ILogger<T> CreateLogger<T>()
    {
        return LoggerFactory.CreateLogger<T>() ?? throw new ArgumentNullException(paramName: nameof(LoggerFactory), message: "LoggerFactory is not initialized.");
    }


    /// <summary>
    /// Получить логер
    /// </summary>
    /// <param name="type"></param>
    /// <returns></returns>
    /// <exception cref="ArgumentNullException"></exception>
    public static ILogger CreateLogger(Type type)
    {
        return LoggerFactory?.CreateLogger(type) ?? throw new ArgumentNullException(paramName: nameof(LoggerFactory), message: "LoggerFactory is not initialized.");
    }
}
