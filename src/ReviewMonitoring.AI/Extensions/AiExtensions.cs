using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using ReviewMonitoring.AI.Client;
using ReviewMonitoring.AI.Consts;
using ReviewMonitoring.AI.Services;
using ReviewMonitoring.Application.Interfaces;

namespace ReviewMonitoring.AI.Extensions;
public static class AiExtensions
{
    public static IServiceCollection AddAi(
    this IServiceCollection services,
    IConfiguration configuration)
    {
        var apiKey = configuration[ConstsAi.Keys.ApiKey];

        if (string.IsNullOrWhiteSpace(apiKey))
        {
            Console.WriteLine("[AI] API key not provided — using mock implementations");
            services.AddScoped<IProductMatcher, MockProductMatcher>();
            services.AddScoped<IReviewAnalyzer, MockReviewAnalyzer>();
            return services;
        }

        Console.WriteLine($"[AI] Using model: {configuration[ConstsAi.Keys.Model] ?? ConstsAi.DefaultModel}");

        var aiConfig = new AiConfig
        {
            ApiKey = apiKey,
            Model = configuration[ConstsAi.Keys.Model] ?? ConstsAi.DefaultModel
        };

        //TODO: Вынести отсюда, сейчас захардкожен
        var analysisConfig = new AnalysisConfig();

        services.AddSingleton(aiConfig);
        services.AddSingleton(analysisConfig);
        services.AddSingleton<IAiClient, AiClient>();
        services.AddScoped<IProductMatcher, ProductMatcher>();
        services.AddScoped<IReviewAnalyzer, ReviewAnalyzer>();

        return services;
    }
}