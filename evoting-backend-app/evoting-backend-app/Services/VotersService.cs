using MongoDB.Driver;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using evoting_backend_app.Models;
using MongoDB.Bson;

namespace evoting_backend_app.Services
{
    public class VotersService
    {
        private readonly IMongoCollection<Voter> voters;

        public VotersService(IEVotingDatabaseSettings settings)
        {
            var client = new MongoClient(settings.ConnectionString);
            var database = client.GetDatabase(settings.DatabaseName);

            voters = database.GetCollection<Voter>(settings.UsersCollectionName);
        }

        public async Task<IEnumerable<Voter>> GetAllVoters()
        {
            return await voters.Find(_ => true).ToListAsync();
        }

        public Task<Voter> GetVoter(string id)
        {
            FilterDefinition<Voter> filter = Builders<Voter>.Filter.Eq(m => m.Id, id);
            return voters.Find(filter).FirstOrDefaultAsync();
        }

        public async Task CreateVoter(Voter voter)
        {
            // ObjectId is generated autmatically if empty Id field is passed
            await voters.InsertOneAsync(voter);
        }

        public async Task<bool> UpdateVoter(Voter voter)
        {
            ReplaceOneResult updateResult = await voters.ReplaceOneAsync(filter: g => g.Id == voter.Id, replacement: voter);
            return updateResult.IsAcknowledged && updateResult.ModifiedCount > 0;
        }

        public async Task<bool> DeleteVoter(string id)
        {
            FilterDefinition<Voter> filter = Builders<Voter>.Filter.Eq(m => m.Id, id);
            DeleteResult deleteResult = await voters.DeleteOneAsync(filter);
            return deleteResult.IsAcknowledged && deleteResult.DeletedCount > 0;
        }

    }
}
