using System.Text.Json.Serialization;

namespace ReviewMonitoring.Ingestion.Ozon.Models;

public class OzonPageResponse
{
    [JsonPropertyName("widgetStates")]
    public Dictionary<string, string> WidgetStates { get; set; } = [];

    [JsonPropertyName("nextPage")]
    public string? NextPage { get; set; }

    [JsonPropertyName("seo")]
    public OzonSeoData? Seo { get; set; }

    [JsonPropertyName("paging")]
    public OzonPaging? Paging { get; set; }
}

public class OzonSeoData
{
    [JsonPropertyName("script")]
    public List<OzonSeoScript> Scripts { get; set; } = [];

    [JsonPropertyName("meta")]
    public List<OzonSeoMeta> Meta { get; set; } = [];
}

public class OzonSeoScript
{
    [JsonPropertyName("innerHTML")]
    public string? InnerHtml { get; set; }

    [JsonPropertyName("type")]
    public string? Type { get; set; }
}

public class OzonSeoMeta
{
    [JsonPropertyName("name")]
    public string? Name { get; set; }

    [JsonPropertyName("content")]
    public string? Content { get; set; }

    [JsonPropertyName("property")]
    public string? Property { get; set; }
}

public class OzonPaging
{
    [JsonPropertyName("total")]
    public int Total { get; set; }

    [JsonPropertyName("page")]
    public int Page { get; set; }

    [JsonPropertyName("perPage")]
    public int PerPage { get; set; }
}