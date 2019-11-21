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
    public class ChannelRepository
    {
        private readonly DynamoDBContext _context;
        public ChannelRepository(AmazonDynamoDBClient client)
        {
            _context = new DynamoDBContext(client);
        }

        public async Task<List<Channel>> GetBotsByChannelId(string id)
        {
            var query = _context.QueryAsync<Channel>(id);

            var botQuery = new Amazon.DynamoDBv2.DocumentModel.QueryOperationConfig
            {
                IndexName = "BotId-ChannelId-index"
            };
            botQuery.KeyExpression = new Amazon.DynamoDBv2.DocumentModel.Expression
            {
                ExpressionStatement = "BotId = :botId",
                ExpressionAttributeValues = new Dictionary<string, Amazon.DynamoDBv2.DocumentModel.DynamoDBEntry>
                {
                    { ":botId", id }
                }
            };
            var botresult = _context.FromQueryAsync<Channel>(botQuery);

            var data = await botresult.GetRemainingAsync();
            var data2 = await query.GetRemainingAsync();

            return data.Union(data2).ToList();
        }
        public Task SaveChannelAsync(Channel channel)
        {
            return _context.SaveAsync(channel);
        }
    }
}
