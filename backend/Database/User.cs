using Amazon.DynamoDBv2.DataModel;

namespace backend.Database
{
    [DynamoDBTable("BCF.User")]
    public class User
    {
        [DynamoDBHashKey] //Partition key
        public string UserId { get; set; }
        [DynamoDBProperty]
        public string ClientId { get; set; }
        [DynamoDBProperty]
        public string Secret { get; set; }
    }
}

//LuckyNoS7evin -> S7evBot

    //S7evbot -> Accept or Decline