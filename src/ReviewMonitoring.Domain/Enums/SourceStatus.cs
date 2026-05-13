// TODO: заменить везде Analyzing на Processing

namespace ReviewMonitoring.Domain.Enums;
public enum SourceStatus
{
    Pending,        // Обработка площадки еще не началась
    Parsing,        // Сбор отзывов
    Analyzing,      // Анализ
    Completed,      // Заверешн
    Failed,         // Конретный сервис (wb, ozon, итд) рухнул и не отвечает или парсинг провалился
    NotFound        // Товара нет на этом источнике
}
