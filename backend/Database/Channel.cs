using Amazon.DynamoDBv2.DataModel;

namespace backend.Database
{
    [DynamoDBTable("BCF.Channel")]
    public class Channel
    {
        [DynamoDBHashKey] //Partition key
        public string ChannelId { get; set; }
        [DynamoDBProperty]
        public string ChannelDisplayName { get; set; }
        [DynamoDBProperty]
        public bool ChannelAccepted { get; set; }
        [DynamoDBRangeKey]
        public string BotId { get; set; }
        [DynamoDBProperty]
        public string BotDisplayName { get; set; }
        [DynamoDBProperty]
        public bool BotAccepted { get; set; }
        [DynamoDBProperty]
        public bool PendingResponse { get; set; }
    }
}
