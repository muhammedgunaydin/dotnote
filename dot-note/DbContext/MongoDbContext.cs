using dot_note.Models;
using MongoDB.Driver;

namespace dot_note.DbContext
{
    public class MongoDbContext
    {
        private readonly IMongoDatabase _database;

        public MongoDbContext(string connString, string dbname)
        {
            var client = new MongoClient(connString);
            _database = client.GetDatabase(dbname);

            var usersCollection = _database.GetCollection<User>("Users");
            var emailIndex = new CreateIndexModel<User>(Builders<User>.IndexKeys.Ascending(u => u.Email), new CreateIndexOptions { Unique = true });
            usersCollection.Indexes.CreateOne(emailIndex);
        }

        public MongoDbContext(IMongoClient mongoClient, string databaseName)
        {
            _database = mongoClient.GetDatabase(databaseName);
        }

        public IMongoCollection<User> Users => _database.GetCollection<User>("Users");
        public IMongoCollection<Note> Notes => _database.GetCollection<Note>("Notes");
    }
}
