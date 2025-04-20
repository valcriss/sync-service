using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml;
using Newtonsoft.Json;

namespace sync_client
{
    public class Config
    {
        private const string FileName = "config.json";

        public string? LocalPath { get; set; } = null;
        public string? ServerUrl { get; set; }
        public int MaxParallelDownloads { get; set; } = 4;

        public static Config LoadOrCreate()
        {
            if (File.Exists(FileName))
            {
                string json = File.ReadAllText(FileName);
                Config? result= JsonConvert.DeserializeObject<Config>(json);
                if (result == null)
                {
                    return new Config();
                }
                return result;
            }
            else
            {
                return new Config();
            }
        }

        public void Save()
        {
            string json = JsonConvert.SerializeObject(this, Newtonsoft.Json.Formatting.Indented);
            File.WriteAllText(FileName, json);
        }
    }
}
