using Microsoft.Extensions.Diagnostics.HealthChecks;
using MongoDB.Bson;

namespace VisionLab.Infrastructure.Persistence;

/// <summary>Usado por /health (balanceadores de carga, Docker, Kubernetes).</summary>
internal sealed class MongoHealthCheck(MongoContext db) : IHealthCheck
{
    public async Task<HealthCheckResult> CheckHealthAsync(HealthCheckContext context, CancellationToken ct = default)
    {
        try
        {
            await db.Database.RunCommandAsync<BsonDocument>(new BsonDocument("ping", 1), cancellationToken: ct);
            return HealthCheckResult.Healthy();
        }
        catch (Exception ex)
        {
            return HealthCheckResult.Unhealthy("MongoDB no responde", ex);
        }
    }
}
