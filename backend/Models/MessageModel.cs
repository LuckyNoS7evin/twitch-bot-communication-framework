using backend.Common;
using System;
using System.Text.Json.Serialization;

namespace backend.Models
{
    public class MessageModel
    {
        [JsonPropertyName("messageId")]
        public Guid MessageId { get; set; }

        [JsonPropertyName("type")]
        [JsonConverter(typeof(JsonStringEnumConverter))]
        public MessageType Type { get; set; }

        [JsonPropertyName("from")]
        public string FromChannelId { get; set; }

        [JsonPropertyName("message")]
        public string MessageString { get; set; }

        [JsonPropertyName("delivered")]
        public bool Delivered { get; set; }

        [JsonPropertyName("expiresAt")]
        public DateTime ExpiresAt { get; set; }

        [JsonPropertyName("createdAt")]
        public DateTime CreatedAt { get; set; }
    }
}
