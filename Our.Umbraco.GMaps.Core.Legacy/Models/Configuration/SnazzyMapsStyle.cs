using Newtonsoft.Json;
using System.Runtime.Serialization;

namespace Our.Umbraco.GMaps.Core.Legacy.Models.Configuration
{
    public class SnazzyMapsStyle
    {
        [DataMember(Name = "json")]
        [JsonProperty("json")]
        public string Json { get; set; }
    }
}