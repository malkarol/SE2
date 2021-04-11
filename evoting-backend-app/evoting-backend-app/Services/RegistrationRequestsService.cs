using MongoDB.Driver;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using evoting_backend_app.Models;
using MongoDB.Bson;

namespace evoting_backend_app.Services
{
    public class RegistrationRequestsService
    {
        private readonly IMongoCollection<RegistrationRequest> registrationRequests;

        public RegistrationRequestsService(IEVotingDatabaseSettings settings)
        {
            var client = new MongoClient(settings.ConnectionString);
            var mainDatabase = client.GetDatabase(settings.MainDatabaseName);

            registrationRequests = mainDatabase.GetCollection<RegistrationRequest>(settings.RegistrationRequestsCollectionName);
        }

        public async Task<IEnumerable<RegistrationRequest>> GetAllRegistrationRequests()
        {
            return await registrationRequests.Find(_ => true).ToListAsync();
        }

        public Task<RegistrationRequest> GetRegistrationRequest(string id)
        {
            FilterDefinition<RegistrationRequest> filter = Builders<RegistrationRequest>.Filter.Eq(m => m.Id, id);
            return registrationRequests.Find(filter).FirstOrDefaultAsync();
        }

        public async Task CreateRegistrationRequest(RegistrationRequest registrationRequest)
        {
            // ObjectId is generated autmatically if empty Id field is passed
            await registrationRequests.InsertOneAsync(registrationRequest);
        }

        public async Task<bool> UpdateRegistrationRequest(RegistrationRequest registrationRequest)
        {
            ReplaceOneResult updateResult = await registrationRequests.ReplaceOneAsync(filter: g => g.Id == registrationRequest.Id, replacement: registrationRequest);
            return updateResult.IsAcknowledged && updateResult.ModifiedCount > 0;
        }

        public async Task<bool> DeleteRegistrationRequest(string id)
        {
            FilterDefinition<RegistrationRequest> filter = Builders<RegistrationRequest>.Filter.Eq(m => m.Id, id);
            DeleteResult deleteResult = await registrationRequests.DeleteOneAsync(filter);
            return deleteResult.IsAcknowledged && deleteResult.DeletedCount > 0;
        }

    }
}
