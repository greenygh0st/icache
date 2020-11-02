using System;
using System.Runtime.InteropServices;

namespace iCache.API
{
    /// <summary>
    /// Global compiled config
    /// </summary>
    public static class Configuration
    {
        public static string RedisConnection {
            get {
                if (string.IsNullOrEmpty(RedisConnectionPassword))
                {
                    return RedisConnectionUri;
                } else
                {
                    // add the redis connection with the password
                    return $"{RedisConnectionUri},password={RedisConnectionPassword}";
                }
            }
        }

        /// <summary>
        /// The URI for the Redis connection. Default: localhost:6379
        /// </summary>
        public static readonly string RedisConnectionUri =
            RuntimeInformation.IsOSPlatform(OSPlatform.Windows) ?
                string.IsNullOrEmpty(Environment.GetEnvironmentVariable("ICACHE_REDIS_URI", EnvironmentVariableTarget.Machine))
                ? "localhost:6379" : Environment.GetEnvironmentVariable("ICACHE_REDIS_URI", EnvironmentVariableTarget.Machine)
            :
                string.IsNullOrEmpty(Environment.GetEnvironmentVariable("ICACHE_REDIS_URI"))
                ? "localhost:6379" : Environment.GetEnvironmentVariable("ICACHE_REDIS_URI")
            ;

        /// <summary>
        /// Redis Password (if exists)
        /// </summary>
        public static readonly string RedisConnectionPassword =
            RuntimeInformation.IsOSPlatform(OSPlatform.Windows) ?
                Environment.GetEnvironmentVariable("ICACHE_REDIS_PASSWORD", EnvironmentVariableTarget.Machine)
            :
                Environment.GetEnvironmentVariable("ICACHE_REDIS_PASSWORD");

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
