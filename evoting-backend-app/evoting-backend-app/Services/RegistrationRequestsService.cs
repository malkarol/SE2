using MongoDB.Driver;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using evoting_backend_app.Models;
using MongoDB.Bson;
using static evoting_backend_app.Utils;
using static evoting_backend_app.Controllers.RegistrationRequestsController;

namespace evoting_backend_app.Services
{
    public class RegistrationRequestsService
    {
        private readonly IMongoDatabase mainDatabase;

        private readonly IMongoDatabase registrationRequestsDatabase;

        private readonly IMongoCollection<Voter> votersCollection;
        private readonly IMongoCollection<Voting> votingsCollection;

        public RegistrationRequestsService(IEVotingDatabaseSettings settings)
        {
            var client = new MongoClient(settings.ConnectionString);

            // Get databases
            mainDatabase = client.GetDatabase(settings.MainDatabaseName);
            registrationRequestsDatabase = client.GetDatabase(settings.RegistrationRequestsDatabaseName);

            // Get collections
            votersCollection = mainDatabase.GetCollection<Voter>(settings.VotersCollectionName);
            votingsCollection = mainDatabase.GetCollection<Voting>(settings.VotingsCollectionName);
        }

        // ---

        public async Task<bool> AddRegistrationRequest(string votingId, RegistrationRequest_Add_DTO registrationRequestAddData)
        {
            // Find registration requests collection
            var registrationRequests = GetCollection<RegistrationRequest>(registrationRequestsDatabase, "voting_" + votingId);

            // Find voting
            var votingFilter = Builders<Voting>.Filter.Eq(o => o.Id, votingId);
            var votingGetTask = votingsCollection.Find(votingFilter).FirstOrDefaultAsync();
            await Task.WhenAll(votingGetTask);
            var voting = votingGetTask.Result;

            // Add new registration request
            var newRegistrationRequest = new RegistrationRequest()
            {
                VoterId = registrationRequestAddData.VoterId,
                VotingId = votingId,
                Comment = registrationRequestAddData.Comment,
                RequestedDate = DateTime.Now,
                Status = voting.FreeRegistration ? RegistrationRequestStatus.Accepted : RegistrationRequestStatus.Pending
            };

            var registrationRequestAddTask = registrationRequests.InsertOneAsync(newRegistrationRequest);
            await Task.WhenAll(registrationRequestAddTask);

            // Add new voting to voter
            var newVotingReference = new VoterVotingReference()
            {
                VotingId = voting.Id,
                Name = voting.Name,
                StartDate = voting.StartDate,
                EndDate = voting.EndDate,
                RegistrationRequestId = newRegistrationRequest.Id,
                RegistrationStatus = newRegistrationRequest.Status,
                RequestedDate = newRegistrationRequest.RequestedDate,
            };

            var voterFilter = Builders<Voter>.Filter.Eq(o => o.Id, registrationRequestAddData.VoterId);
            var votingReferencesUpdate = Builders<Voter>.Update.Push<VoterVotingReference>(o => o.VotingReferences, newVotingReference);
            var votingReferencesUpdateTask = votersCollection.UpdateOneAsync(voterFilter, votingReferencesUpdate);

            await Task.WhenAll(votingReferencesUpdateTask);
            var votingReferencesUpdateResult = votingReferencesUpdateTask.Result;

            return votingReferencesUpdateResult.IsAcknowledged && votingReferencesUpdateResult.ModifiedCount > 0;
        }

        public async Task<bool> RegistrationRequestDecide(string votingId, string registrationRequestId, RegistrationRequest_Decision_DTO registrationRequestDecisionData)
        {
            var registrationRequestsCollection = GetCollection<RegistrationRequest>(registrationRequestsDatabase, "voting_" + votingId);

            var registrationRequestFilter = Builders<RegistrationRequest>.Filter.Eq(o => o.Id, registrationRequestId);

            var registrationRequestUpdate = Builders<RegistrationRequest>.Update
                .Set("ResolvedDate", DateTime.Now)
                .Set("Status", registrationRequestDecisionData.Decision)
                .Set("CoordinatorId", registrationRequestDecisionData.CoordinatorId);

            var registrationRequestUpdateTask = registrationRequestsCollection.UpdateOneAsync(registrationRequestFilter, registrationRequestUpdate);
            await Task.WhenAll(registrationRequestUpdateTask);
            var registrationRequestUpdateR = registrationRequestUpdateTask.Result;

            // TODO: Replace this with advanced query that will update and return updated (like in utils)
            var registrationRequestGetTask = registrationRequestsCollection.Find(registrationRequestFilter).FirstOrDefaultAsync();
            await Task.WhenAll(registrationRequestGetTask);
            var registrationRequest = registrationRequestGetTask.Result;

            var voterFilter = Builders<Voter>.Filter.Eq(o => o.Id, registrationRequest.VoterId);
            var voterGetTask = votersCollection.Find(voterFilter).FirstOrDefaultAsync();
            await Task.WhenAll(voterGetTask);
            var voter = voterGetTask.Result;

            var votingReferenceFilter = Builders<Voter>.Filter.And(
                Builders<Voter>.Filter.Eq(o => o.Id, registrationRequest.VoterId),
                Builders<Voter>.Filter.ElemMatch(o => o.VotingReferences, f => f.VotingId == votingId));

            var votingReferenceUpdate = Builders<Voter>.Update
                .Set(o => o.VotingReferences[-1].RegistrationStatus, (RegistrationRequestStatus)((int)registrationRequestDecisionData.Decision));

            var votingReferencesTask = votersCollection.UpdateOneAsync(votingReferenceFilter, votingReferenceUpdate);
            await Task.WhenAll(votingReferencesTask);

            return true;
        }

        public async Task<PagedList<RegistrationRequest_BasicInfo_DTO>> GetVotingRegistrationRequests(string votingId, RegistrationRequest_BasicInfo_QueryParameters queryParameters)
        {
            var registrationRequestsCollection = GetCollection<RegistrationRequest>(registrationRequestsDatabase, "voting_" + votingId);

            // Filtering
            var registrationRequestsFilterBuilder = Builders<RegistrationRequest>.Filter;
            var registrationRequestsFilter = registrationRequestsFilterBuilder.Empty;         
            if (queryParameters.Status != null)
                registrationRequestsFilter = registrationRequestsFilter & registrationRequestsFilterBuilder.Eq(o => o.Status, queryParameters.Status);

            // Sorting
            var registrationRequestsSort = Builders<RegistrationRequest>.Sort.Ascending(o => o.Status);

            // Finding and paging
            var registrationRequestsGetTask = registrationRequestsCollection.AggregateByPage(registrationRequestsFilter, registrationRequestsSort, queryParameters.PageNumber, queryParameters.PageSize);
            await Task.WhenAll(registrationRequestsGetTask);
            var registrationRequests = registrationRequestsGetTask.Result;

            // TODO: Maybe some cleanup using projecting?
            var registrationRequestsBasicInfo = registrationRequests.data.ConvertAll(o => new RegistrationRequest_BasicInfo_DTO { Id = o.Id, RequestedDate = o.RequestedDate, ResolvedDate = o.ResolvedDate, Status = o.Status });

            var registrationRequestsPage = new PagedList<RegistrationRequest_BasicInfo_DTO>(registrationRequestsBasicInfo, registrationRequests.totalItemCount, queryParameters.PageNumber, queryParameters.PageSize);

            return registrationRequestsPage;

        }

        public async Task<RegistrationRequest> GetRegistrationRequest(string votingId, string registrationRequestId)
        {
            var registrationRequestsCollection = GetCollection<RegistrationRequest>(registrationRequestsDatabase, "voting_" + votingId);
            var registrationRequestFilter = Builders<RegistrationRequest>.Filter.Eq(o => o.Id, registrationRequestId);
            var registrationRequestGetTask = registrationRequestsCollection.Find(registrationRequestFilter).FirstOrDefaultAsync();
            await Task.WhenAll(registrationRequestGetTask);
            var registrationRequest = registrationRequestGetTask.Result;

            return registrationRequest;
        }

    }
}
