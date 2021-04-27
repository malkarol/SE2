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
        private readonly IMongoDatabase mainDatabase;
        private readonly IMongoDatabase votesDatabase;
        private readonly IMongoDatabase registrationRequestsDatabase;

        private readonly IMongoCollection<Voter> votersCollection;
        private readonly IMongoCollection<Voting> votingsCollection;


        public VotersService(IEVotingDatabaseSettings settings)
        {
            var client = new MongoClient(settings.ConnectionString);

            // Get databases
            mainDatabase = client.GetDatabase(settings.MainDatabaseName);
            votesDatabase = client.GetDatabase(settings.VotesDatabaseName);
            registrationRequestsDatabase = client.GetDatabase(settings.RegistrationRequestsDatabaseName);

            // Get collections
            votersCollection = mainDatabase.GetCollection<Voter>(settings.VotersCollectionName);
            votingsCollection = mainDatabase.GetCollection<Voting>(settings.VotingsCollectionName);
        }

        private IMongoCollection<Vote> GetVotesCollection(string votingId)
        {
            return votesDatabase.GetCollection<Vote>(votingId);
        }

        private IMongoCollection<RegistrationRequest> GetRegistrationRequestsCollection(string votingId)
        {
            return registrationRequestsDatabase.GetCollection<RegistrationRequest>(votingId);
        }

        // ---

        public async Task<Voter_BasicInfo_DTO> GetVoter(string id)
        {
            FilterDefinition<Voter> filter = Builders<Voter>.Filter.Eq(o => o.Id, id);
            var voterTask = votersCollection.Find(filter).FirstOrDefaultAsync();
            await Task.WhenAll(voterTask);
            var voter = voterTask.Result;

            // Create DTO
            var result = new Voter_BasicInfo_DTO()
            {
                FirstName = voter.FirstName,
                LastName = voter.LastName,
                Email = voter.Email
            };

            return result;
        }


    }
}
