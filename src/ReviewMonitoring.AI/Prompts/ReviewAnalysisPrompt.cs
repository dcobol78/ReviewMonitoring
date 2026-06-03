
using ReviewMonitoring.Domain.Models;

namespace ReviewMonitoring.AI.Prompts;
internal static class ReviewAnalysisPrompt
{
    public static string Analyze(IReadOnlyList<Review> reviews)
    {
        var reviewsText = string.Join("\n---\n",
            reviews.Select(r => $"Rating: {r.Rating}/5\n{r.Text}"));

        return $$"""
            Analyze the following product reviews. Return ONLY valid JSON without markdown.

            Format:
            {
              "pros": ["pro 1", "pro 2"],
              "cons": ["con 1", "con 2"],
              "keywords": ["keyword 1", "keyword 2"],
              "flags": ["possible_counterfeit", "shipping_issues"],
              "summary": "brief summary in 2-3 sentences"
            }

            Available flags: possible_counterfeit, shipping_issues, quality_issues, fake_reviews, size_issues.
            Include flags only if there's clear evidence in the reviews.

            Reviews:
            {{reviewsText}}
            """;
    }
}
