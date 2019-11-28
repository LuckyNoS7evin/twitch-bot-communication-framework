using Amazon.DynamoDBv2.DataModel;
using backend.Common;
using System;

namespace backend.Database
{
    [DynamoDBTable("BCF.Message")]
    public class Message
    {
        [DynamoDBHashKey] //Partition key
        public Guid MessageId { get; set; }
        [DynamoDBProperty]
        public string ChannelId { get; set; }
        [DynamoDBProperty]
        public string FromChannelId { get; set; }
        [DynamoDBProperty]
        public MessageType Type { get; set; }
        [DynamoDBProperty]
        public string MessageString { get; set; }
        [DynamoDBProperty]
        public bool Delivered { get; set; }
        [DynamoDBProperty]
        public DateTime ExpiresAt { get; set; }
        [DynamoDBProperty]
        public DateTime CreatedAt { get; set; }
    }
}
