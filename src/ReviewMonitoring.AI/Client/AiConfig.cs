namespace ReviewMonitoring.AI.Client;

//Можно будет добавить fallback model, но это опционально и на будущее
public class AiConfig
{
    public required string ApiKey { get; set; }
    public required string Model { get; set; }
}
