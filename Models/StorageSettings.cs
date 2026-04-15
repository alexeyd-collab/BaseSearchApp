namespace SearchApp.Models
{
    public class StorageSettings
    {
        public StorageType Provider { get; set; } = StorageType.Memory;

        /// <summary>
        /// Redis connection string. Required when Provider = DistributedCache.
        /// Example: "localhost:6379" or "redis:6379,password=secret"
        /// </summary>
        public string? RedisConnectionString { get; set; }

        /// <summary>
        /// How long search results are cached. Defaults to 10 minutes.
        /// </summary>
        public int CacheExpirationMinutes { get; set; } = 10;
    }

    public enum StorageType
    {
        Memory,
        DistributedCache
    }
}
