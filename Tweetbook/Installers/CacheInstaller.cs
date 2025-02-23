
using Tweetbook.Cache;
using Tweetbook.Services;

namespace Tweetbook.Installers
{
    public class CacheInstaller : IInstaller
    {
        // NOTE: We need to run Redis Cache. Docker Command below:
        // docker run -p 6379:6379 redis
        public void InstallServices(IServiceCollection services, IConfiguration configuration)
        {
            var redisCacheSettings = new RedisCacheSettings();
            configuration.GetSection(nameof(RedisCacheSettings)).Bind(redisCacheSettings);
            services.AddSingleton(redisCacheSettings);

            if(!redisCacheSettings.Enabled)
            {
                return;
            }

            services.AddStackExchangeRedisCache(options => options.Configuration = redisCacheSettings.ConnectionString);
            services.AddSingleton<IResponseCacheService, ResponseCacheService>();
        }
    }
}
