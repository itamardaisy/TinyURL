using TinyUrl.Models;
using MongoDB.Driver;

namespace TinyUrl.Data
{
    public class MongoDbContext
    {
        private readonly IMongoDatabase _database;

        public MongoDbContext(string connectionString, string databaseName)
        {
            var client = new MongoClient(connectionString);
            _database = client.GetDatabase(databaseName);
        }

        public IMongoCollection<UrlMapping> UrlMappings => _database.GetCollection<UrlMapping>("UrlMappings");
    }
}