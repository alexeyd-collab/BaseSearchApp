namespace SearchApp.Models
{
    public class StorageSettings
    {
        public StorageType Provider { get; set; } = StorageType.Memory;
    }

    public enum StorageType
    {
        Memory,
        DistributedCache
    }
}
