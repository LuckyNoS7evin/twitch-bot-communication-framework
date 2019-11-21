using backend.Common;
using Newtonsoft.Json;
using Newtonsoft.Json.Converters;
using System.Collections.Generic;

namespace backend.Models
{
    public class MessageModel
    {
        [JsonProperty("channelIds")]
        public List<string> ChannelIds { get; set; }

        [JsonProperty("type")]
        [JsonConverter(typeof(StringEnumConverter))]
        public MessageType Type { get; set; }

        [JsonProperty("message")]
        public string MessageString { get; set; }
    }
}
