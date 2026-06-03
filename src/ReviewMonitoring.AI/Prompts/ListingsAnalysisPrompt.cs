using ReviewMonitoring.Application.Models;
using System.Text;

namespace ReviewMonitoring.AI.Prompts;
public class ListingsAnalysisPrompt
{
    public static string AnalyzeBatch(
        IReadOnlyList<(string Index, ListingReviews Listing)> indexed)
    {
        var sb = new StringBuilder();

        sb.AppendLine("Ты анализируешь отзывы покупателей на один и тот же товар у разных продавцов на маркетплейсах.");
        sb.AppendLine("Проанализируй отзывы КАЖДОГО продавца отдельно (по его индексу), затем дай общий вывод по товару в целом.");
        sb.AppendLine("Выявляй достоинства, недостатки, ключевые слова и тревожные флаги: брак, обман, несоответствие описанию, подозрение на накрутку отзывов.");
        sb.AppendLine("Если у продавца высокий рейтинг, но в отзывах преобладают жалобы — отметь это во flags.");
        sb.AppendLine();
        sb.AppendLine("Выбери ЛУЧШЕГО продавца для покупки (поле bestSeller — его индекс, например \"L2\").");
        sb.AppendLine("Учитывай соотношение цены, рейтинга, количества отзывов и их содержания, отсутствие тревожных флагов.");
        sb.AppendLine("В summary лучшего продавца объясни, ПОЧЕМУ именно он лучший по сравнению с остальными.");
        sb.AppendLine();
        sb.AppendLine("Верни ТОЛЬКО валидный JSON без markdown и пояснений в формате:");
        sb.AppendLine("""
            {
              "listings": [
                { "index": "L1", "pros": [], "cons": [], "keywords": [], "flags": [], "summary": "" }
              ],
              "overall": { "pros": [], "cons": [], "keywords": [], "flags": [], "summary": "" },
              "bestSeller": "L1"
            }
            """);
        sb.AppendLine();
        sb.AppendLine("Данные по продавцам:");

        foreach (var (index, lr) in indexed)
        {
            var l = lr.ListingData;
            sb.AppendLine($"=== {index} ===");
            sb.AppendLine($"Продавец: {l.SellerName}");
            sb.AppendLine($"Товар: {l.ProductTitle}");
            sb.AppendLine($"Цена: {l.Price} | Рейтинг: {l.Rating} | Отзывов всего: {l.ReviewCount}");
            sb.AppendLine($"Показано отзывов (выборка): {lr.Reviews.Count}");

            foreach (var r in lr.Reviews)
            {
                var text = string.IsNullOrWhiteSpace(r.Text) ? "(без текста)" : r.Text;
                sb.AppendLine($"[{r.Rating}/5] {text}");
            }
            sb.AppendLine();
        }

        return sb.ToString();
    }
}
