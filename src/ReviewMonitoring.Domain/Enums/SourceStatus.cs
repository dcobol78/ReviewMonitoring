namespace ReviewMonitoring.Domain.Enums;
public enum SourceStatus
{
    Pending,
    Parsing,
    Completed,
    Failed,      // Конретный сервис (wb, ozon, итд) рухнул и не отвечает или парсинг провалился
    NotFound     // Товара нет на этом источнике
}
