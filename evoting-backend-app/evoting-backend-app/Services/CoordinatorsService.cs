using MongoDB.Driver;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using evoting_backend_app.Models;
using MongoDB.Bson;

namespace evoting_backend_app.Services
{
    public class CoordinatorsService
    {
        private readonly IMongoCollection<Coordinator> coordinators;

        public CoordinatorsService(IEVotingDatabaseSettings settings)
        {
            var client = new MongoClient(settings.ConnectionString);
            var database = client.GetDatabase(settings.DatabaseName);

            coordinators = database.GetCollection<Coordinator>(settings.CoordinatorsCollectionName);
        }

        public async Task<IEnumerable<Coordinator>> GetAllCoordinators()
        {
            return await coordinators.Find(_ => true).ToListAsync();
        }

        public Task<Coordinator> GetCoordinator(string id)
        {
            FilterDefinition<Coordinator> filter = Builders<Coordinator>.Filter.Eq(m => m.Id, id);
            return coordinators.Find(filter).FirstOrDefaultAsync();
        }

        public async Task CreateCoordinator(Coordinator voter)
        {
            // ObjectId is generated autmatically if empty Id field is passed
            await coordinators.InsertOneAsync(voter);
        }

        public async Task<bool> UpdateCoordinator(Coordinator coordinator)
        {
            ReplaceOneResult updateResult = await coordinators.ReplaceOneAsync(filter: g => g.Id == coordinator.Id, replacement: coordinator);
            return updateResult.IsAcknowledged && updateResult.ModifiedCount > 0;
        }

        public async Task<bool> DeleteCoordinator(string id)
        {
            FilterDefinition<Coordinator> filter = Builders<Coordinator>.Filter.Eq(m => m.Id, id);
            DeleteResult deleteResult = await coordinators.DeleteOneAsync(filter);
            return deleteResult.IsAcknowledged && deleteResult.DeletedCount > 0;
        }

    }
}
