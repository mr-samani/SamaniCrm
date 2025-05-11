using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SamaniCrm.Infrastructure.Cache
{
    public class CacheSettings
    {
        public string Provider { get; set; } = "Memory";
        public RedisOptions Redis { get; set; } = new();
        public FileOptions File { get; set; } = new();
    }

    public class RedisOptions
    {
        public string ConnectionString { get; set; } = string.Empty;
    }

    public class FileOptions
    {
        public string BasePath { get; set; } = "CacheFiles";
    }

    public enum CacheProvider
    {
        Memory,
        Redis,
        File
    }

}
