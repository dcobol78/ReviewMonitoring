// Models/WbSearchResponse.cs
using System.Text.Json.Serialization;

namespace ReviewMonitoring.Ingestion.Wildberries.Models;

public class WbSearchResponse
{
    [JsonPropertyName("data")] public WbSearchData? Data { get; set; }
}

public class WbSearchData
{
    [JsonPropertyName("products")] public List<WbSearchProduct> Products { get; set; } = [];
}

public class WbSearchProduct
{
    [JsonPropertyName("id")] public long Id { get; set; }
    [JsonPropertyName("name")] public string Name { get; set; } = string.Empty;
}

public class WbFeedbacksResponse
{
    [JsonPropertyName("feedbackCount")] public int FeedbackCount { get; set; }

    [JsonPropertyName("valuation")] public string? Valuation { get; set; }

    [JsonPropertyName("valuationDistribution")]
    public Dictionary<string, int> ValuationDistribution { get; set; } = [];

    [JsonPropertyName("feedbacks")] public List<WbFeedback> Feedbacks { get; set; } = [];
}

public class WbFeedback
{
    [JsonPropertyName("id")] public string Id { get; set; } = string.Empty;
    [JsonPropertyName("nmId")] public long NmId { get; set; }
    [JsonPropertyName("text")] public string Text { get; set; } = string.Empty;
    [JsonPropertyName("pros")] public string Pros { get; set; } = string.Empty;
    [JsonPropertyName("cons")] public string Cons { get; set; } = string.Empty;
    [JsonPropertyName("productValuation")] public int ProductValuation { get; set; }
    [JsonPropertyName("createdDate")] public DateTime CreatedDate { get; set; }
    [JsonPropertyName("wbUserDetails")] public WbUserDetails? WbUserDetails { get; set; }
    [JsonPropertyName("votes")] public WbVotes? Votes { get; set; }
}

public class WbUserDetails
{
    [JsonPropertyName("name")] public string Name { get; set; } = string.Empty;
}

public class WbVotes
{
    [JsonPropertyName("pluses")] public int Pluses { get; set; }
}

public class WbCardResponse
{
    [JsonPropertyName("products")]
    public List<WbProduct> Products { get; set; } = [];
}

public class WbProduct
{
    [JsonPropertyName("id")] public long Id { get; set; }
    [JsonPropertyName("root")] public long Root { get; set; }   // imtId
    [JsonPropertyName("name")] public string Name { get; set; } = string.Empty;
    [JsonPropertyName("supplier")] public string Supplier { get; set; } = string.Empty;
    [JsonPropertyName("supplierRating")] public decimal SupplierRating { get; set; }
    [JsonPropertyName("reviewRating")] public decimal ReviewRating { get; set; }
    [JsonPropertyName("feedbacks")] public int Feedbacks { get; set; }
    [JsonPropertyName("sizes")] public List<WbSize> Sizes { get; set; } = [];
}

public class WbSize
{
    [JsonPropertyName("price")] public WbPrice? Price { get; set; }
}

public class WbPrice
{
    [JsonPropertyName("product")] public long Product { get; set; }  // в копейках
}