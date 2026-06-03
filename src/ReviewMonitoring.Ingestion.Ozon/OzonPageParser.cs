// Ingestion.Ozon/OzonPageParser.cs
using ReviewMonitoring.Application.Models;
using ReviewMonitoring.Domain.Models;
using ReviewMonitoring.Ingestion.Ozon.Models;
using ReviewMonitoring.Shared.Helpers;
using System.Text.Json;

namespace ReviewMonitoring.Ingestion.Ozon;

internal static class OzonPageParser
{
    // Парсим данные товара из JSON-LD в seo.script
    public static OzonProductData? ParseProductData(OzonPageResponse page)
    {
        var jsonLd = ParseJsonLd(page);
        if (jsonLd is null)
            return ParseFromWidgetStates(page);
        return ParseFromJsonLd(page, jsonLd);
    }

    private static OzonProductData ParseFromJsonLd(OzonPageResponse page, Dictionary<string, JsonElement> jsonLd)
    {
        var data = new OzonProductData
        {
            Title = jsonLd.TryGetValue("name", out var name)
                ? name.GetString() ?? string.Empty
                : string.Empty,

            Sku = jsonLd.TryGetValue("sku", out var sku)
                ? sku.GetString() ?? string.Empty
                : string.Empty
        };

        if (jsonLd.TryGetValue("offers", out var offers))
            data.Price = decimal.TryParse(
                offers.GetProperty("price").GetString(),
                System.Globalization.NumberStyles.Any,
                System.Globalization.CultureInfo.InvariantCulture,
                out var p) ? p : 0;

        if (jsonLd.TryGetValue("aggregateRating", out var rating))
        {
            data.Rating = decimal.TryParse(
                rating.GetProperty("ratingValue").GetString(),
                System.Globalization.NumberStyles.Any,
                System.Globalization.CultureInfo.InvariantCulture,
                out var r) ? r : 0;

            data.ReviewCount = int.TryParse(
                rating.GetProperty("reviewCount").GetString(),
                out var rc) ? rc : 0;
        }

        // Изображение из og:image в meta
        var ogImage = page.Seo?.Meta?
            .FirstOrDefault(m => m.Property == "og:image")?.Content;
        if (ogImage is not null)
            data.ImageUrls = [ogImage];

        // Продавец и другие продавцы из widgetStates
        ParseWidgetStates(page, data);

        return data;
    }

    // Парсим отзывы из widgetStates
    public static (List<Review> Reviews, string? NextPage) ParseReviewsPage(
        OzonPageResponse page,
        string listingUrl)
    {
        var reviews = new List<Review>();

        foreach (var (key, value) in page.WidgetStates)
        {
            if (!key.StartsWith("webListReviews-"))
                continue;

            try
            {
                var state = JsonDocument.Parse(value).RootElement;
                if (!state.TryGetProperty("reviews", out var reviewsArray))
                    continue;

                foreach (var r in reviewsArray.EnumerateArray())
                {
                    var review = TryParseReview(r, listingUrl);
                    if (review is not null)
                        reviews.Add(review);
                }
            }
            catch { }
        }

        return (reviews, page.NextPage);
    }

    // Парсим URL других продавцов
    public static List<string> ParseOtherSellerUrls(OzonPageResponse page)
    {
        var urls = new List<string>();

        foreach (var (_, value) in page.WidgetStates)
        {
            try
            {
                var state = JsonDocument.Parse(value).RootElement;
                if (!state.TryGetProperty("items", out var items))
                    continue;

                foreach (var item in items.EnumerateArray())
                {
                    if (item.TryGetProperty("action", out var action) &&
                        action.TryGetProperty("link", out var link))
                    {
                        var url = NormalizeUrl(link.GetString());
                        if (!string.IsNullOrEmpty(url))
                            urls.Add(url);
                    }
                }
            }
            catch { }
        }

        return urls
            .Where(u => u.Contains("/product/"))
            .Distinct()
            .ToList();
    }

    public static List<ProductCandidate> ParseSearchResults(OzonPageResponse page)
    {
        var candidates = new List<ProductCandidate>();

        foreach (var (key, value) in page.WidgetStates)
        {
            if (!key.StartsWith("tileGridDesktop-"))
                continue;

            try
            {
                var state = JsonDocument.Parse(value).RootElement;
                if (!state.TryGetProperty("items", out var items) ||
                    items.ValueKind != JsonValueKind.Array)
                    continue;

                foreach (var item in items.EnumerateArray())
                {
                    var candidate = TryParseSearchItem(item);
                    if (candidate is not null)
                        candidates.Add(candidate);
                }
            }
            catch { }
        }

        return candidates;
    }

    public static string? ParseSearchNextPage(OzonPageResponse page)
    {
        foreach (var (key, value) in page.WidgetStates)
        {
            if (!key.StartsWith("infiniteVirtualPaginator-"))
                continue;

            try
            {
                var state = JsonDocument.Parse(value).RootElement;
                if (state.TryGetProperty("nextPage", out var next))
                    return next.GetString();
            }
            catch { }
        }

        return null;
    }

    private static void ParseWidgetStates(OzonPageResponse page, OzonProductData data)
    {
        foreach (var (_, value) in page.WidgetStates)
        {
            try
            {
                var state = JsonDocument.Parse(value).RootElement;

                // Продавец
                if (state.TryGetProperty("seller", out var seller))
                {
                    if (seller.TryGetProperty("name", out var sn))
                        data.SellerName = sn.GetString() ?? string.Empty;
                    if (seller.TryGetProperty("link", out var sl))
                        data.SellerUrl = sl.GetString() ?? string.Empty;
                }

                // Другие продавцы
                if (state.TryGetProperty("modalLink", out var modalLink))
                    data.OtherSellersModalUrl = modalLink.GetString();

                // Изображения
                if (state.TryGetProperty("images", out var images) &&
                    data.ImageUrls.Count == 0)
                {
                    data.ImageUrls = images.EnumerateArray()
                        .Where(img => img.TryGetProperty("src", out _))
                        .Select(img => img.GetProperty("src").GetString() ?? string.Empty)
                        .Where(url => !string.IsNullOrEmpty(url))
                        .ToList();
                }
            }
            catch { }
        }
    }

    private static Review? TryParseReview(JsonElement r, string listingUrl)
    {
        try
        {
            var content = r.GetProperty("content");
            var author = r.GetProperty("author");

            var textParts = new List<string>();

            if (content.TryGetProperty("comment", out var comment) &&
                !string.IsNullOrWhiteSpace(comment.GetString()))
                textParts.Add(comment.GetString()!);

            if (content.TryGetProperty("positive", out var positive) &&
                !string.IsNullOrWhiteSpace(positive.GetString()))
                textParts.Add("Достоинства: " + positive.GetString());

            if (content.TryGetProperty("negative", out var negative) &&
                !string.IsNullOrWhiteSpace(negative.GetString()))
                textParts.Add("Недостатки: " + negative.GetString());

            var likes = 0;
            if (r.TryGetProperty("usefulness", out var usefulness) &&
                usefulness.TryGetProperty("useful", out var useful))
                likes = useful.GetInt32();

            return new Review
            {
                Id = r.GetProperty("uuid").GetString() ?? Guid.NewGuid().ToString(),
                ListingUrl = listingUrl,
                Text = string.Join(" ", textParts), // пустая строка если нет текста
                Rating = content.GetProperty("score").GetInt32(),
                AuthorName = author.GetProperty("firstName").GetString() ?? "Аноним",
                Date = DateTimeOffset.FromUnixTimeSeconds(
                    r.GetProperty("publishedAt").GetInt64()).UtcDateTime,
                Likes = likes
            };
        }
        catch
        {
            return null;
        }
    }

    private static Dictionary<string, JsonElement>? ParseJsonLd(OzonPageResponse page)
    {
        var script = page.Seo?.Scripts?
            .FirstOrDefault(s => s.Type == "application/ld+json");

        if (script?.InnerHtml is null)
            return null;

        try
        {
            return JsonDocument.Parse(script.InnerHtml)
                .RootElement
                .EnumerateObject()
                .ToDictionary(p => p.Name, p => p.Value);
        }
        catch { return null; }
    }

    private static string? NormalizeUrl(string? url)
    {
        if (string.IsNullOrEmpty(url))
            return null;

        url = UrlHelper.NormalizeUrl(url);
        if (url.StartsWith("/"))
            url = "https://www.ozon.ru" + url;

        return url.Contains("ozon.ru") ? url : null;
    }

    private static ProductCandidate? TryParseSearchItem(JsonElement item)
    {
        try
        {
            // Ссылка из action.link
            if (!item.TryGetProperty("action", out var action) ||
                !action.TryGetProperty("link", out var link))
                return null;

            var url = NormalizeUrl(link.GetString());
            if (string.IsNullOrEmpty(url) || !url.Contains("/product/"))
                return null;

            // Название из mainState[] где id == "name"
            var title = string.Empty;
            if (item.TryGetProperty("mainState", out var mainState) &&
                mainState.ValueKind == JsonValueKind.Array)
            {
                foreach (var state in mainState.EnumerateArray())
                {
                    if (state.TryGetProperty("type", out var type) &&
                        type.GetString() == "textAtom" &&
                        state.TryGetProperty("id", out var id) &&
                        id.GetString() == "name" &&
                        state.TryGetProperty("textAtom", out var textAtom) &&
                        textAtom.TryGetProperty("text", out var text))
                    {
                        title = text.GetString() ?? string.Empty;
                        break;
                    }
                }
            }

            if (string.IsNullOrEmpty(title))
                return null;

            return new ProductCandidate
            {
                Url = url,
                Title = title,
                ProviderName = "Ozon"
            };
        }
        catch
        {
            return null;
        }
    }

    private static OzonProductData? ParseFromWidgetStates(OzonPageResponse page)
    {
        foreach (var (key, value) in page.WidgetStates)
        {
            if (!key.StartsWith("webCharacteristics"))
                continue;

            try
            {
                var state = JsonDocument.Parse(value).RootElement;

                if (!state.TryGetProperty("products", out var products))
                    continue;

                // Берём первый товар из products
                var firstProduct = products.EnumerateObject().FirstOrDefault();
                if (firstProduct.Value.ValueKind == JsonValueKind.Undefined)
                    continue;

                var product = firstProduct.Value;

                var data = new OzonProductData
                {
                    Title = product.TryGetProperty("name", out var name)
                        ? name.GetString() ?? string.Empty
                        : string.Empty,
                };

                if (product.TryGetProperty("coverImage", out var cover))
                    data.ImageUrls = [cover.GetString() ?? string.Empty];

                // Рейтинг из productScore
                if (state.TryGetProperty("productScore", out var score))
                    data.Rating = (decimal)score.GetDouble();

                // Paging для количества отзывов
                if (state.TryGetProperty("paging", out var paging) &&
                    paging.TryGetProperty("total", out var total))
                    data.ReviewCount = total.GetInt32();

                // Продавец и другие данные из остальных widgetStates
                ParseWidgetStates(page, data);

                return data;
            }
            catch { }
        }

        return null;
    }
}