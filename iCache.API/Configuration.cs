using System;
using System.Runtime.InteropServices;

namespace iCache.API
{
    public static class Configuration
    {
        /// <summary>
        /// The URI for the Redis connection. Default: localhost:6379
        /// </summary>
        public static readonly string RedisConnection =
            RuntimeInformation.IsOSPlatform(OSPlatform.Windows) ?
                string.IsNullOrEmpty(Environment.GetEnvironmentVariable("ICACHE_REDIS_URI", EnvironmentVariableTarget.Machine))
                ? "localhost:6379" : Environment.GetEnvironmentVariable("ICACHE_REDIS_URI", EnvironmentVariableTarget.Machine)
            :
                string.IsNullOrEmpty(Environment.GetEnvironmentVariable("ICACHE_REDIS_URI"))
                ? "localhost:6379" : Environment.GetEnvironmentVariable("ICACHE_REDIS_URI")
            ;
        /// <summary>
        /// Admin user client id
        /// </summary>
        public static readonly string AdminUserClient =
            RuntimeInformation.IsOSPlatform(OSPlatform.Windows) ? 
                Environment.GetEnvironmentVariable("ICACHE_ADMIN_USER", EnvironmentVariableTarget.Machine)
            :
                Environment.GetEnvironmentVariable("ICACHE_ADMIN_USER");
        /// <summary>
        /// Admin password
        /// </summary>
        public static readonly string AdminPassword =
            RuntimeInformation.IsOSPlatform(OSPlatform.Windows) ?
                Environment.GetEnvironmentVariable("ICACHE_ADMIN_PASSWORD", EnvironmentVariableTarget.Machine)
            :
                Environment.GetEnvironmentVariable("ICACHE_ADMIN_PASSWORD")
            ;
    }
}
