using Newtonsoft.Json;

namespace ElvUINET.Utils.Config
{
    [JsonObject]
    public class Configuration
    {
        [JsonProperty("WowPath")]
        public string WowPath;
    }
}
