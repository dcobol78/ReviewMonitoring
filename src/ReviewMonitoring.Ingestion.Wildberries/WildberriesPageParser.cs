using ReviewMonitoring.Application.Models;
using ReviewMonitoring.Domain.Models;
using ReviewMonitoring.Ingestion.Wildberries.Models;

namespace ReviewMonitoring.Ingestion.Wildberries;

internal static class WildberriesPageParser
{
    public static List<Review> ParseFeedbacks(
        WbFeedbacksResponse response, long nmId, string listingUrl)
    {
        var reviews = new List<Review>();

        foreach (var fb in response.Feedbacks)
        {
            // У WB отзывы могут быть на разные nmId внутри одного imtId — фильтруем
            if (fb.NmId != 0 && fb.NmId != nmId)
                continue;

            reviews.Add(new Review
            {
                Id = fb.Id,
                ListingUrl = listingUrl,
                Text = BuildText(fb),
                Rating = fb.ProductValuation,
                Date = fb.CreatedDate,
                AuthorName = fb.WbUserDetails?.Name,
                Likes = fb.Votes?.Pluses ?? 0
            });
        }

        return reviews;
    }

    // Текст = comment + достоинства + недостатки
    private static string BuildText(WbFeedback fb)
    {
        var parts = new List<string>();

        if (!string.IsNullOrWhiteSpace(fb.Text))
            parts.Add(fb.Text);
        if (!string.IsNullOrWhiteSpace(fb.Pros))
            parts.Add($"Достоинства: {fb.Pros}");
        if (!string.IsNullOrWhiteSpace(fb.Cons))
            parts.Add($"Недостатки: {fb.Cons}");

        return string.Join("\n", parts);
    }

    public static Dictionary<int, int> ParseDistribution(WbFeedbacksResponse response)
    {
        var result = new Dictionary<int, int>();
        foreach (var (key, count) in response.ValuationDistribution)
            if (int.TryParse(key, out var rating) && count > 0)
                result[rating] = count;
        return result;
    }

    public static List<ProductCandidate> ParseSearch(WbSearchResponse response)
    {
        if (response.Data is null)
            return [];

        return response.Data.Products
            .Select(p => new ProductCandidate
            {
                Title = p.Name,
                Url = BuildProductUrl(p.Id),
                ProviderName = "Wildberries"
            })
            .ToList();
    }

    public static string BuildProductUrl(long nmId) =>
        $"https://www.wildberries.ru/catalog/{nmId}/detail.aspx";

    // Изображения по nmId через basket-хост
    public static List<string> BuildImageUrls(long nmId, int picsCount)
    {
        var vol = nmId / 100000;
        var part = nmId / 1000;
        var basket = GetBasketHost(vol);

        var urls = new List<string>();
        for (var i = 1; i <= Math.Min(picsCount, 10); i++)
            urls.Add($"https://{basket}.wbbasket.ru/vol{vol}/part{part}/{nmId}/images/big/{i}.webp");

        return urls;
    }

    // Маппинг vol → номер basket-хоста (диапазоны WB)
    private static string GetBasketHost(long vol) => vol switch
    {
        <= 143 => "basket-01",
        <= 287 => "basket-02",
        <= 431 => "basket-03",
        <= 719 => "basket-04",
        <= 1007 => "basket-05",
        <= 1061 => "basket-06",
        <= 1115 => "basket-07",
        <= 1169 => "basket-08",
        <= 1313 => "basket-09",
        <= 1601 => "basket-10",
        <= 1655 => "basket-11",
        <= 1919 => "basket-12",
        <= 2045 => "basket-13",
        <= 2189 => "basket-14",
        <= 2405 => "basket-15",
        <= 2621 => "basket-16",
        <= 2837 => "basket-17",
        _ => "basket-18"
    };
}