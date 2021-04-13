using System.Collections.Generic;
using System.Threading.Tasks;
using evoting_backend_app.Models;
using Microsoft.Extensions.Logging;
using MongoDB.Driver;

namespace evoting_backend_app.Services
{
    

    public class UserService 
    {
        private readonly ILogger<UserService> logger;

        private readonly IMongoCollection<User> users;

        public UserService(IEVotingDatabaseSettings settings, ILogger<UserService> logger )
        {
            this.logger = logger;
            var client = new MongoClient(settings.ConnectionString);
            var mainDatabase = client.GetDatabase(settings.MainDatabaseName);

            users = mainDatabase.GetCollection<User>(settings.VotersCollectionName);
        }

        public async Task<IEnumerable<User>> GetAllVoters()
        {
            return await users.Find(_ => true).ToListAsync();
        }

        public Task<User> GetUser(string userName)
        {
            FilterDefinition<User> filter = Builders<User>.Filter.Eq(m => m.UserName, userName);
            return users.Find(filter).FirstOrDefaultAsync();
        }

        public async Task CreateUser(User user)
        {
            // ObjectId is generated autmatically if empty Id field is passed
            await users.InsertOneAsync(user);
        }

        public async Task<bool> UpdateUser(User user)
        {
            ReplaceOneResult updateResult = await users.ReplaceOneAsync(filter: g => g.UserName == user.UserName, replacement: user);
            return updateResult.IsAcknowledged && updateResult.ModifiedCount > 0;
        }

        public async Task<bool> DeleteUser(string userName)
        {
            FilterDefinition<User> filter = Builders<User>.Filter.Eq(m => m.UserName, userName);
            DeleteResult deleteResult = await users.DeleteOneAsync(filter);
            return deleteResult.IsAcknowledged && deleteResult.DeletedCount > 0;
        }
        public bool IsValidUserCredentials(string userName, string password)
        {
            this.logger.LogInformation($"Validating user [{userName}]");
            if (string.IsNullOrWhiteSpace(userName))
            {
                return false;
            }

            if (string.IsNullOrWhiteSpace(password))
            {
                return false;
            }
            var p = GetUser(userName).Result.Password;
            
            return  p == password;
        }

        public bool IsAnExistingUser(string userName)
        {
            return GetUser(userName).Result == null ? true : false;
        }

        public string GetUserRole(string userName)
        {
            if (!IsAnExistingUser(userName))
            {
                return string.Empty;
            }

            if (userName == "admin")
            {
                return UserRoles.Admin;
            }

            return UserRoles.BasicUser;
        }
    }

    public static class UserRoles
    {
        public const string Admin = nameof(Admin);
        public const string BasicUser = nameof(BasicUser);
    }

}
    
        

       

