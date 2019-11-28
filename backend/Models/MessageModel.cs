using backend.Common;
using Newtonsoft.Json;
using Newtonsoft.Json.Converters;
using System;

namespace backend.Models
{
    public class MessageModel
    {
        [JsonProperty("messageId")]
        public Guid MessageId { get; set; }

        [JsonProperty("type")]
        [JsonConverter(typeof(StringEnumConverter))]
        public MessageType Type { get; set; }

        [JsonProperty("from")]
        public string FromChannelId { get; set; }

        [JsonProperty("message")]
        public string MessageString { get; set; }

        [JsonProperty("delivered")]
        public bool Delivered { get; set; }

        [JsonProperty("expiresAt")]
        public DateTime ExpiresAt { get; set; }

        [JsonProperty("createdAt")]
        public DateTime CreatedAt { get; set; }
    }
}
