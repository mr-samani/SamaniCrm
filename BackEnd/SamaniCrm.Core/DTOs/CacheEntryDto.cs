using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SamaniCrm.Core.Shared.DTOs
{
    public class CacheEntryDto
    {
        public string Key { get; set; } = default!;
        public string Provider { get; set; } = default!;
        public DateTime? LastModified { get; set; }
        public TimeSpan? Expiration { get; set; }
        public long? SizeInBytes { get; set; }
    }
}
