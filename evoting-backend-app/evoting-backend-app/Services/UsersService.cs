using MongoDB.Driver;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using evoting_backend_app.Models;
using MongoDB.Bson;

namespace evoting_backend_app.Services
{
    public class UsersService
    {
        private readonly IMongoCollection<User> _users;

        public UsersService(IEVotingDatabaseSettings settings)
        {
            var client = new MongoClient(settings.ConnectionString);
            var database = client.GetDatabase(settings.DatabaseName);

            _users = database.GetCollection<User>(settings.UsersCollectionName);
        }

        public async Task<IEnumerable<User>> GetAllUsers()
        {
            return await _users
                            .Find(_ => true)
                            .ToListAsync();
        }

        public Task<User> GetUser(string id)
        {
            FilterDefinition<User> filter = Builders<User>.Filter.Eq(m => m.Id, id);
            return _users
                    .Find(filter)
                    .FirstOrDefaultAsync();
        }

        public async Task CreateUser(UserDTO_Post user)
        {


            await _users.InsertOneAsync(new User(user));
        }

        public async Task<bool> UpdateUser(User user)
        {
            ReplaceOneResult updateResult =
                await _users
                        .ReplaceOneAsync(
                            filter: g => g.Id == user.Id,
                            replacement: user);
            return updateResult.IsAcknowledged
                    && updateResult.ModifiedCount > 0;
        }

        public async Task<bool> DeleteUser(string id)
        {
            FilterDefinition<User> filter = Builders<User>.Filter.Eq(m => m.Id, id);
            DeleteResult deleteResult = await _users
                                              .DeleteOneAsync(filter);
            return deleteResult.IsAcknowledged
                && deleteResult.DeletedCount > 0;
        }

    }
}
