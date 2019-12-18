using backend.Common;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Text.Json.Serialization;

namespace backend.Models
{
    public class MessageRequestModel
    {
        [Required]
        [JsonPropertyName("channelIds")]
        public List<string> ChannelIds { get; set; }

        [Required]
        [JsonPropertyName("type")]
        [JsonConverter(typeof(JsonStringEnumConverter))]
        public MessageType Type { get; set; }

        [Required]
        [JsonPropertyName("message")]
        public string MessageString { get; set; }
    }
}
