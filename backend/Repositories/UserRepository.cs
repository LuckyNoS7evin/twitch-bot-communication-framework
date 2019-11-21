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
    public class UserRepository
    {
        private readonly DynamoDBContext _context;
        public UserRepository(AmazonDynamoDBClient client)
        {
            _context = new DynamoDBContext(client);
        }

        public Task<User> GetUserAsync(string id)
        {
            return _context.LoadAsync<User>(id);
        }

        public Task<List<User>> GetUsersByClientIdAsync(string id)
        {
            var userQuery = new Amazon.DynamoDBv2.DocumentModel.QueryOperationConfig
            {
                IndexName = "ClientId-index"
            };
            userQuery.KeyExpression = new Amazon.DynamoDBv2.DocumentModel.Expression
            {
                ExpressionStatement = "ClientId = :clientId",
                ExpressionAttributeValues = new Dictionary<string, Amazon.DynamoDBv2.DocumentModel.DynamoDBEntry>
                {
                    { ":clientId", id }
                }
            };
            var botresult = _context.FromQueryAsync<User>(userQuery);
            return botresult.GetRemainingAsync();
        }

        public Task SaveUserAsync(User user)
        {
            return _context.SaveAsync(user);
        }
    }
}
