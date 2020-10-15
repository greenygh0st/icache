using System;
using System.Runtime.InteropServices;

namespace iCache.API
{
    public static class OperatingSystem
    {
        public static bool IsWindows() =>
            RuntimeInformation.IsOSPlatform(OSPlatform.Windows);

        public static bool IsMacOS() =>
            RuntimeInformation.IsOSPlatform(OSPlatform.OSX);

        public static bool IsLinux() =>
            RuntimeInformation.IsOSPlatform(OSPlatform.Linux);
    }

    public static class Configuration
    {
        public static readonly string RedisConnection = Environment.GetEnvironmentVariable("ICACHE_REDIS_URI");
        public static readonly string AdminUserClient = Environment.GetEnvironmentVariable("ICACHE_ADMIN_USER");
        public static readonly string AdminPassword = Environment.GetEnvironmentVariable("ICACHE_ADMIN_PASSWORD");
    }
}
