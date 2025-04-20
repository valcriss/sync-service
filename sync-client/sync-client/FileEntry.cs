using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Newtonsoft.Json;

namespace sync_client
{
    public class FileEntry
    {
        [JsonProperty("path")]
        public string Path { get; set; }
        [JsonProperty("hash")]
        public string Hash { get; set; }
        [JsonProperty("size")]
        public long Size { get; set; }
    }
}
