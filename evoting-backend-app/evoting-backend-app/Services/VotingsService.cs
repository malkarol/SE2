using MongoDB.Driver;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using evoting_backend_app.Models;
using MongoDB.Bson;
using static evoting_backend_app.Controllers.VotingsController;
using static evoting_backend_app.Utils;

namespace evoting_backend_app.Services
{
    public class VotingsService
    {
        private readonly IMongoDatabase mainDatabase;
        private readonly IMongoDatabase votesDatabase;
        private readonly IMongoDatabase registrationRequestsDatabase;

        private readonly IMongoCollection<Coordinator> coordinatorsCollection;
        private readonly IMongoCollection<Voter> votersCollection;
        private readonly IMongoCollection<Voting> votingsCollection;

        public VotingsService(IEVotingDatabaseSettings settings)
        {
            var client = new MongoClient(settings.ConnectionString);

            // Get databases
            mainDatabase = client.GetDatabase(settings.MainDatabaseName);
            votesDatabase = client.GetDatabase(settings.VotesDatabaseName);
            registrationRequestsDatabase = client.GetDatabase(settings.RegistrationRequestsDatabaseName);

            // Get collections
            votersCollection = mainDatabase.GetCollection<Voter>(settings.VotersCollectionName);
            votingsCollection = mainDatabase.GetCollection<Voting>(settings.VotingsCollectionName);
            coordinatorsCollection = mainDatabase.GetCollection<Coordinator>(settings.CoordinatorsCollectionName);
        }

        // ---

        public async Task<PagedList<Voting>> GetAllVotings(Voting_BasicInfo_QueryParameters queryParameters)
        {
            // Filtering
            var votingsFilter = Builders<Voting>.Filter.Empty;

            if (queryParameters.Name != null)
                votingsFilter = votingsFilter & Builders<Voting>.Filter.Regex(o => o.Name, queryParameters.Name);
            if (queryParameters.RangeStart != null && queryParameters.RangeEnd != null)
                votingsFilter = votingsFilter & Builders<Voting>.Filter.Where(o => (o.StartDate <= queryParameters.RangeEnd && queryParameters.RangeStart <= o.EndDate ));

            // Sorting
            var votingsSorter = Builders<Voting>.Sort.Ascending(o => o.Name);

            // Finding and paging
            var votingsGetTask = votingsCollection.AggregateByPage(votingsFilter, votingsSorter, queryParameters.PageNumber, queryParameters.PageSize);
            await Task.WhenAll(votingsGetTask);
            var votings = votingsGetTask.Result;

            var votingsPage = new PagedList<Voting>(votings.data, votings.totalItemCount, queryParameters.PageNumber, queryParameters.PageSize);

            return votingsPage;
        }

        public async Task<Voting> GetVoting(string votingId)
        {
            var votingFilter = Builders<Voting>.Filter.Eq(o => o.Id, votingId);
            var votingGetTask = votingsCollection.Find(votingFilter).FirstOrDefaultAsync();
            await Task.WhenAll(votingGetTask);
            var voting = votingGetTask.Result;

            return voting;
        }

        public async Task<Voting_Voter_DTO> GetVotingVoter(string votingId, string voterId)
        {
            // TODO: Convert it to aggregation query on database side
            // TODO2: Use registrationRequestId instead of id, this will limit number of featches from 3 to 1 (no need voter and searching in VotingReferences list)
            var voterFilterBuilder = Builders<Voter>.Filter;
            var voterFilter = voterFilterBuilder.Eq(o => o.Id, voterId);
            var voterGetTask = votersCollection.Find(voterFilter).FirstOrDefaultAsync();

            var votingFilterBuilder = Builders<Voting>.Filter;
            var votingFilter = votingFilterBuilder.Eq(o => o.Id, votingId);
            var votingGetTask = votingsCollection.Find(votingFilter).FirstOrDefaultAsync();

            await Task.WhenAll(voterGetTask, votingGetTask);
            var voter = voterGetTask.Result; 
            var voting = votingGetTask.Result;

            var votingReference = voter.VotingReferences.Find(o => o.VotingId == votingId);

            var voteFilterBuilder = Builders<Vote>.Filter;
            var voteFilter = voteFilterBuilder.Eq(o => o.Id, votingReference.VoteId);
            var voteGetTask = GetCollection<Vote>(votesDatabase, "voting_" + votingId).Find(voteFilter).FirstOrDefaultAsync();
            await Task.WhenAll(voteGetTask);
            var vote = voteGetTask.Result;

            var result = new Voting_Voter_DTO()
            {
                Id = voting.Id,
                Name = voting.Name,
                Description = voting.Description,
                StartDate = voting.StartDate,
                EndDate = voting.EndDate,
                Options = voting.Options,

                VoteDate = vote.VoteDate,
                VotingOptionNumbers = vote.VotingOptionNumbers
            };

            return result;
        }

        public async Task<Voting> AddVoting(Voting_Add_DTO votingAddData)
        {
            var newVoting = new Voting()
            {
                Name = votingAddData.Name,
                Description = votingAddData.Description,
                StartDate = votingAddData.StartDate,
                EndDate = votingAddData.EndDate,
                Options = votingAddData.Options,
                FreeRegistration = votingAddData.FreeRegistration,
                Report = null,
                CoordinatorReferences = new List<CoordinatorReference>()
            };
        
            var votingAddTask = votingsCollection.InsertOneAsync(newVoting);
            await Task.WhenAll(votingAddTask);

            return newVoting;
        }

        public async Task<Voting> UpdateVoting(string votingId, Voting_Update_DTO votingUpdateData)
        {
            // TODO: Add transaction here {
            var votingFilter = Builders<Voting>.Filter.Eq(o => o.Id, votingId);

            // TODO: Detect what changes and what and then skip updating duplicated data in other entries if possible
            var votingUpdate = Builders<Voting>.Update
                .Set("Name", votingUpdateData.Name)
                .Set("Description", votingUpdateData.Description)
                .Set("StartDate", votingUpdateData.StartDate)
                .Set("EndDate", votingUpdateData.EndDate)
                .Set("Options", votingUpdateData.Options)
                .Set("FreeRegistration", votingUpdateData.FreeRegistration);

            var votingUpdateTask = votingsCollection.UpdateOneAsync(votingFilter, votingUpdate);
            await Task.WhenAll(votingUpdateTask);
            var votingUpdateR = votingUpdateTask.Result;

            // TODO: Replace this with advanced query that will update and return updated (like in utils)
            var votingGetTask = votingsCollection.Find(votingFilter).FirstOrDefaultAsync();
            await Task.WhenAll(votingGetTask);
            var voting = votingGetTask.Result;

            // WARNING: Extremely inefficient code
            // TODO: Go through whole collection of registration requests, via each go to voter and in it find VoterVotingReference and update it, do it in chunks and in aggregation
            var registrationRequestsCollection = GetCollection<RegistrationRequest>(registrationRequestsDatabase, "voting_" + votingId);
            var registrationRequestsCollectionSize = registrationRequestsCollection.CountDocuments(new BsonDocument());
            for (int i = 0; i < registrationRequestsCollectionSize; i++)
            {
                var registrationRequestFilter = Builders<RegistrationRequest>.Filter.Empty;

                var registrationRequest = registrationRequestsCollection.Find(registrationRequestFilter).Skip(i + 1).Limit(1).FirstOrDefault();

                var voterId = registrationRequest.VoterId;
                var voterVotingReferenceFilter = Builders<Voter>.Filter.Where(o => o.Id == voterId && o.VotingReferences.Any(c => c.VotingId == votingId));
                var voterVotingReferenceUpdate = Builders<Voter>.Update
                                .Set(o => o.VotingReferences[-1].Name, votingUpdateData.Name)
                                .Set(o => o.VotingReferences[-1].StartDate, votingUpdateData.StartDate)
                                .Set(o => o.VotingReferences[-1].EndDate, votingUpdateData.EndDate)
                                ;
                var voterVotingReferenceUpdateTask = votersCollection.FindOneAndUpdateAsync(voterVotingReferenceFilter, voterVotingReferenceUpdate);
                await Task.WhenAll(voterVotingReferenceUpdateTask); // TODO: Store and check tasks in list or bulk write?
            }

            // Update data in CoordinatorVotingReference in each coordinator
            for (int i = 0; i < voting.CoordinatorReferences.Count; i++)
            {
                // Get coordinator
                // Update its CoordinatorVotingReference
            }

            // TODO: Add transaction here }

            return voting;
        }

        public async Task<bool> Vote(string votingId, Vote_Ballot_DTO voteBallot)
        {
            //TODO: Add transaction
            var newVote = new Vote()
            {
                VotingId = votingId,
                VoterId = voteBallot.VoterId,
                VoteDate = DateTime.Now,
                VotingOptionNumbers = voteBallot.VotingOptionNumbers
            };

            var votesCollection = GetCollection<Vote>(votesDatabase, "voting_" + votingId);
            var voteTask = votesCollection.InsertOneAsync(newVote);

            var voterFilterBuilder = Builders<Voter>.Filter;
            var voterFilter = voterFilterBuilder.Eq(o => o.Id, voteBallot.VoterId);
            var voterGetTask = votersCollection.Find(voterFilter).FirstOrDefaultAsync();

            await Task.WhenAll(voterGetTask, voteTask);
            var voter = voterGetTask.Result;

            //var votingReferenceIndex = voter.VotingReferences.FindIndex(o => o.VotingId == id);

            var votingReferenceFilter = Builders<Voter>.Filter.And(
                Builders<Voter>.Filter.Eq(o => o.Id, voteBallot.VoterId),
                Builders<Voter>.Filter.ElemMatch(o => o.VotingReferences, f => f.VotingId == votingId));

            var votingReferenceUpdate = Builders<Voter>.Update
                .Set(o => o.VotingReferences[-1].VoteId, newVote.Id)
                .Set(o => o.VotingReferences[-1].VoteDate, newVote.VoteDate);

            var votingReferencesTask = votersCollection.UpdateOneAsync(votingReferenceFilter, votingReferenceUpdate);
            await Task.WhenAll(votingReferencesTask);

            return true;
        }

        public async Task<bool> AddCoordinatorToVoting(string votingId, Voting_AddCoordinator_DTO votingAddCoordinatorData)
        {
            var coordinatorFilter = Builders<Coordinator>.Filter.Eq(o => o.Id, votingId);
            var coordinatorGetTask = coordinatorsCollection.Find(coordinatorFilter).FirstOrDefaultAsync();          
            var coordinator = coordinatorGetTask.Result;
            await Task.WhenAll(coordinatorGetTask);

            // Add new entry to coordinator references of voting
            var newCoordinatorReference = new CoordinatorReference()
            {
                CoordinatorId = votingAddCoordinatorData.CoordinatorId,
                CoordinatorFirstName = coordinator.FirstName,
                CoordinatorLastName = coordinator.LastName
            };
            var votingFilter = Builders<Voting>.Filter.Eq(o => o.Id, votingId);
            var votingCoordinatorReferencesUpdate = Builders<Voting>.Update.Push<CoordinatorReference>(o => o.CoordinatorReferences, newCoordinatorReference);
            var votingCoordinatorReferencesUpdateTask = votingsCollection.UpdateOneAsync(votingFilter, votingCoordinatorReferencesUpdate);

            // Add new entry to voting references of coordinator
            var newCoordinatorVotingReference = new CoordinatorVotingReference()
            {
                VotingId = votingId,
                Comment = votingAddCoordinatorData.Comment
            };
            var coordinatorVotingReferenceUpdate = Builders<Coordinator>.Update.Push<CoordinatorVotingReference>(o => o.VotingReferences, newCoordinatorVotingReference);
            var coordinatorVotingReferenceUpdateTask = coordinatorsCollection.UpdateOneAsync(coordinatorFilter, coordinatorVotingReferenceUpdate);

            await Task.WhenAll(votingCoordinatorReferencesUpdateTask, coordinatorVotingReferenceUpdateTask);

            return true;
        }
    }
}
