namespace ReviewMonitoring.Core.Enums;
public enum SourceJobStatus
{
    Pending,
    Parsing,
    Completed,
    Failed,      // Конретный сервис (wb, ozon, итд) рухнул и не отвечает или парсинг провалился
    NotFound     // Товара нет на этом источнике
}
