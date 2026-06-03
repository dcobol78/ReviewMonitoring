using ReviewMonitoring.Application.Models;
using ReviewMonitoring.Domain.Enums;
using System.Text.Json;

namespace ReviewMonitoring.AI.Prompts;
internal static class ProductMatchPrompt
{
    public static string ExtractSearchTerms(string title) =>
        $$"""
        Generate alternative search queries for finding this product across e-commerce platforms.
        The "exact" field must be the original title unchanged.
        The "synonyms" field must contain alternative shorter or reformulated search queries.
        Return ONLY valid JSON without markdown, without explanations.
        Format:
        {
          "exact": "{{title}}",
          "synonyms": ["alternative query 1", "alternative query 2"]
        }
        Title: {{title}}
        """;

    public static string FilterMatches(
        string originalTitle,
        List<ProductCandidate> candidates,
        MatchStrictness strictness,
        int maxResults)
    {
        var strictnessDescription = strictness switch
        {
            MatchStrictness.Loose => "similar products (different colors, sizes, variants are OK)",
            MatchStrictness.Medium => "same product, possibly from different sellers",
            MatchStrictness.Strict => "exact same product only",
            _ => "same product"
        };

        var candidatesJson = JsonSerializer.Serialize(
            candidates.Select((c, i) => new { index = i, c.Title, c.ProviderName }));

        return $$"""
            From the list of candidates, select up to {{maxResults}} that match the original product.
            Match strictness: {{strictnessDescription}}.

            Return ONLY valid JSON array of indices, without markdown, without explanations.
            Example: [0, 3, 5]

            Original product: {{originalTitle}}

            Candidates:
            {{candidatesJson}}
            """;
    }
}
