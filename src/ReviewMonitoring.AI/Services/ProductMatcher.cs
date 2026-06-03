using Microsoft.Extensions.Logging;
using ReviewMonitoring.AI.Client;
using ReviewMonitoring.AI.Prompts;
using ReviewMonitoring.Application.Interfaces;
using ReviewMonitoring.Application.Models;
using ReviewMonitoring.Domain.Models;

namespace ReviewMonitoring.AI.Services;
internal class ProductMatcher : IProductMatcher
{
    private readonly IAiClient _ai;
    private readonly ILogger<ProductMatcher> _log;

    public ProductMatcher(IAiClient ai, ILogger<ProductMatcher> log)
    {
        _ai = ai;
        _log = log;
    }

    public async Task<ProductSearchTerms> ExtractSearchTermsAsync(string title, CancellationToken ct)
    {
        var prompt = ProductMatchPrompt.ExtractSearchTerms(title);

        var result = await _ai.CompleteStructuredAsync<ProductSearchTerms>(prompt, ct);

        if (result is null)
        {
            _log.LogWarning("Failed to extract search terms for {Title} — using title as-is", title);
            return new ProductSearchTerms { Exact = title };
        }

        return result;
    }

    public async Task<List<ProductCandidate>> FilterMatchesAsync(
        string originalTitle,
        List<ProductCandidate> candidates,
        IngestionConfig config,
        CancellationToken ct)
    {
        if (candidates.Count == 0)
            return [];

        var prompt = ProductMatchPrompt.FilterMatches(
            originalTitle, candidates, config.Strictness, config.MaxFinalMatches);

        var indices = await _ai.CompleteStructuredAsync<List<int>>(prompt, ct);

        if (indices is null || indices.Count == 0)
        {
            _log.LogWarning("LLM returned no matches — falling back to first {Count}",
                config.MaxFinalMatches);
            return candidates.Take(config.MaxFinalMatches).ToList();
        }

        return indices
            .Where(i => i >= 0 && i < candidates.Count)
            .Select(i => candidates[i])
            .ToList();
    }
}