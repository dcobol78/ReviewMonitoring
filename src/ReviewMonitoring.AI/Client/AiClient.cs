using Microsoft.Extensions.Logging;
using OpenAI;
using OpenAI.Chat;
using ReviewMonitoring.AI.Client;
using ReviewMonitoring.AI.Consts;
using System.ClientModel;
using System.Text.Json;

public class AiClient : IAiClient
{
    private readonly ChatClient _client;
    private readonly ILogger<AiClient> _log;

    public AiClient(AiConfig config, ILogger<AiClient> log)
    {
        var options = new OpenAIClientOptions
        {
            Endpoint = new Uri(ConstsAi.OpenRouterEndpoint)
        };

        var openAiClient = new OpenAIClient(
            new ApiKeyCredential(config.ApiKey),
            options);

        _client = openAiClient.GetChatClient(config.Model);
        _log = log;
    }

    public async Task<string> CompleteAsync(string prompt, CancellationToken ct)
    {
        _log.LogInformation("AI request, prompt length: {Length}", prompt.Length);

        try
        {
            var response = await _client.CompleteChatAsync(
                [new UserChatMessage(prompt)],
                cancellationToken: ct);

            var text = response.Value.Content[0].Text;
            _log.LogInformation("AI response received, length: {Length}", text.Length);

            return text;
        }
        catch (Exception ex)
        {
            _log.LogError(ex, "AI request failed");
            throw;
        }
    }

    public async Task<T?> CompleteStructuredAsync<T>(string prompt, CancellationToken ct)
    {
        var response = await CompleteAsync(prompt, ct);

        try
        {
            var result = JsonSerializer.Deserialize<T>(response,
                new JsonSerializerOptions { PropertyNameCaseInsensitive = true });

            _log.LogInformation("AI structured response parsed as {Type}", typeof(T).Name);
            return result;
        }
        catch (Exception ex)
        {
            _log.LogWarning(ex, "Failed to parse AI response as {Type}. Raw: {Raw}",
                typeof(T).Name,
                response.Length > 200 ? response[..200] : response);
            return default;
        }
    }
}