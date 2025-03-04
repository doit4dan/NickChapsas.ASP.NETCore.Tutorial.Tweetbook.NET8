﻿using Microsoft.Extensions.Diagnostics.HealthChecks;
using StackExchange.Redis;

namespace Tweetbook.HealthChecks
{
    public class RedisHealthCheck : IHealthCheck
    {
        private readonly IConnectionMultiplexer _connectionMultiplexer;

        public RedisHealthCheck(IConnectionMultiplexer connectionMultiplexer)
        {
            _connectionMultiplexer = connectionMultiplexer;
        }

        public Task<HealthCheckResult> CheckHealthAsync(HealthCheckContext context, CancellationToken cancellationToken = default)
        {
            try
            {
                var database = _connectionMultiplexer.GetDatabase(); // attempts to obtain connection
                database.StringGet("health"); // attempts to read from db
                return Task.FromResult(HealthCheckResult.Healthy());
            }
            catch (Exception exception)
            {
                return Task.FromResult(HealthCheckResult.Unhealthy(exception.Message)); // would not recommended passing exception details in live system as it may contain sensitive details
            }
        }
    }
}
