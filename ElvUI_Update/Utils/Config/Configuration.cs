using Newtonsoft.Json;

namespace ElvUI_Update.Utils.Config
{
    [JsonObject]
    public class Configuration
    {
        [JsonProperty("WowPath")]
        public string WowPath;
    }
}
