using MongoDB.Driver;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using evoting_backend_app.Models;
using MongoDB.Bson;

namespace evoting_backend_app.Services
{
    public class VotingsService
    {
        private readonly IMongoCollection<Voting> votings;

        public VotingsService(IEVotingDatabaseSettings settings)
        {
            var client = new MongoClient(settings.ConnectionString);
            var database = client.GetDatabase(settings.DatabaseName);

            votings = database.GetCollection<Voting>(settings.VotingsCollectionName);
        }

        public async Task<IEnumerable<Voting>> GetAllVotings()
        {
            return await votings.Find(_ => true).ToListAsync();
        }

        public Task<Voting> GetVoting(string id)
        {
            FilterDefinition<Voting> filter = Builders<Voting>.Filter.Eq(m => m.Id, id);
            return votings.Find(filter).FirstOrDefaultAsync();
        }

        public async Task CreateVoting(Voting voting)
        {
            // ObjectId is generated autmatically if empty Id field is passed
            await votings.InsertOneAsync(voting);
        }

        public async Task<bool> UpdateVoting(Voting voting)
        {
            ReplaceOneResult updateResult = await votings.ReplaceOneAsync(filter: g => g.Id == voting.Id, replacement: voting);
            return updateResult.IsAcknowledged && updateResult.ModifiedCount > 0;
        }

        public async Task<bool> DeleteVoting(string id)
        {
            FilterDefinition<Voting> filter = Builders<Voting>.Filter.Eq(m => m.Id, id);
            DeleteResult deleteResult = await votings.DeleteOneAsync(filter);
            return deleteResult.IsAcknowledged && deleteResult.DeletedCount > 0;
        }

    }
}
