using Amazon.DynamoDBv2;
using Amazon.DynamoDBv2.DataModel;
using Amazon.DynamoDBv2.Model;
using backend.Database;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace backend.Repositories
{
    public class MessageRepository
    {
        private readonly DynamoDBContext _context;
        public MessageRepository(AmazonDynamoDBClient client)
        {
            _context = new DynamoDBContext(client);
        }
        
        public Task<List<Message>> GetMessagesByChannelIdAsync(string id)
        {
            var userQuery = new Amazon.DynamoDBv2.DocumentModel.QueryOperationConfig
            {
                IndexName = "ChannelId-index"
            };
            userQuery.KeyExpression = new Amazon.DynamoDBv2.DocumentModel.Expression
            {
                ExpressionStatement = "ChannelId = :channelId",
                ExpressionAttributeValues = new Dictionary<string, Amazon.DynamoDBv2.DocumentModel.DynamoDBEntry>
                {
                    { ":channelId", id }
                }
            };
            var botresult = _context.FromQueryAsync<Message>(userQuery);
            return botresult.GetRemainingAsync();
        }

        public Task SaveMessageAsync(Message message)
        {
            return _context.SaveAsync(message);
        }
    }
}
