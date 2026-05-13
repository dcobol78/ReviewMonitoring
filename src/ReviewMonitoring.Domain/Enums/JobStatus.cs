using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ReviewMonitoring.Domain.Enums;
public enum JobStatus
{
    Pending,            // Задача создана, но этап сбора еще не начат
    Parsing,            // Сбор отзывов
    Parsed,             // Отзывы собраны
    Analyzing,          // Анализ отзывов
    Completed,          // Задача завершена
    PartiallyCompleted, // Задача завершена частично
    Failed,             // Задача не завершена
    Cancelled,          // Задача отменена
    Paused              // Пока бесполезно, да и вообще наверное не надо, перезапустить задачу будет логичнее, не? Пусть пока будет.
}
