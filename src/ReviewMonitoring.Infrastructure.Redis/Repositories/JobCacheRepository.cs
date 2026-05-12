using ReviewMonitoring.Application.Interfaces;
using ReviewMonitoring.Domain.Domain;
using StackExchange.Redis;
using System.Text.Json;

namespace ReviewMonitoring.Infrastructure.Redis.Repositories;

public class JobCacheRepository : IJobCacheRepository
{
    private readonly IDatabase _db;
    private readonly TimeSpan _ttl = TimeSpan.FromHours(24);

    public JobCacheRepository(IConnectionMultiplexer redis)
    {
        _db = redis.GetDatabase();
    }

    private static string Key(Guid id) => $"job:{id}";

    public async Task SetAsync(Job job)
    {
        string json = JsonSerializer.Serialize(job);
        await _db.StringSetAsync(Key(job.Id), json);
        await _db.KeyExpireAsync(Key(job.Id), _ttl);
    }

    public async Task<Job?> GetByIdAsync(Guid id)
    {
        RedisValue value = await _db.StringGetAsync(Key(id));
        if (value.IsNullOrEmpty)
            return null;
        return JsonSerializer.Deserialize<Job>(value.ToString());
    }

    public async Task DeleteAsync(Guid id)
    {
        await _db.KeyDeleteAsync(Key(id));
    }

    public async Task PingAsync(Guid id)
    {
        await _db.KeyExpireAsync(Key(id), _ttl);
    }
}