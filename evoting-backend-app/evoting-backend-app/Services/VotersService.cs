using MongoDB.Driver;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using evoting_backend_app.Models;
using MongoDB.Bson;
using static evoting_backend_app.Controllers.VotersController;
using System.Linq.Expressions;
using static evoting_backend_app.PredicateExtensions;
using System.Reflection;
using static evoting_backend_app.Utils;

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

        // ---
   
        public async Task<PagedList<Voter>> GetAllVoters(Voter_BasicInfo_QueryParameters queryParameters) // Debug only
        {
            // Filtering
            var votersFilterBuilder = Builders<Voter>.Filter;
            var votersFilter = votersFilterBuilder.Empty;
            if (queryParameters.FirstName != null)
                votersFilter = votersFilter & votersFilterBuilder.Regex(o => o.FirstName, queryParameters.FirstName);
            if (queryParameters.LastName != null)
                votersFilter = votersFilter & votersFilterBuilder.Regex(o => o.LastName, queryParameters.LastName);

            // Sorting
            var votersSort = Builders<Voter>.Sort.Ascending(o => o.FirstName);

            // Finding and paging
            var votersGetTask = votersCollection.AggregateByPage(votersFilter, votersSort, queryParameters.PageNumber, queryParameters.PageSize);
            await Task.WhenAll(votersGetTask);
            var voters = votersGetTask.Result;

            var votersPage = new PagedList<Voter>(voters.data, voters.totalItemCount, queryParameters.PageNumber, queryParameters.PageSize);

            return votersPage;
        }

        public async Task<Voter_BasicInfo_DTO> GetVoter(string voterId)
        {        
            var voterFilter = Builders<Voter>.Filter.Eq(o => o.Id, voterId);
            var voterGetTask = votersCollection.Find(voterFilter).FirstOrDefaultAsync();
            await Task.WhenAll(voterGetTask);
            var voter = voterGetTask.Result;

            var result = new Voter_BasicInfo_DTO()
            {
                Id = voter.Id,
                FirstName = voter.FirstName,
                LastName = voter.LastName,
                Email = voter.Email
            };

            return result;
        }

        public async Task<PagedList<VoterVotingReference>> GetVoterVotings(string voterId, Voter_Voting_BasicInfo_QueryParameters queryParameters)
        {
            // Find voter
            var voterFilter = Builders<Voter>.Filter.Eq(o => o.Id, voterId);
            var getVoterTask = votersCollection.Find(voterFilter).FirstOrDefaultAsync();
            await Task.WhenAll(getVoterTask);
            var voter = getVoterTask.Result;

            
            // Filtering   
            Predicate<VoterVotingReference> votingsFilter = (o => o != null);
            if (queryParameters.Name != null)
                votingsFilter = votingsFilter + (o => o.Name == queryParameters.Name);
            if (queryParameters.StartDate != null)
                votingsFilter = votingsFilter + (o => o.StartDate >= queryParameters.StartDate);
            if (queryParameters.EndDate != null)
                votingsFilter = votingsFilter + (o => o.EndDate <= queryParameters.EndDate);
            if (queryParameters.Active != null)
                votingsFilter = votingsFilter + (o => DateTime.Now >= o.StartDate && DateTime.Now <= o.EndDate);
            if (queryParameters.RegistrationStatus != null)
                votingsFilter = votingsFilter + (o => o.RegistrationStatus == queryParameters.RegistrationStatus);
            if (queryParameters.AlreadyVoted != null)
                votingsFilter = votingsFilter + (o => o.VoteDate != null);


            //var votingsFilterBuilder = Builders<VoterVotingReference>.Filter;
            //var votingsFilter = votingsFilterBuilder.Empty;
            //if (queryParameters.Name != null)
            //    votingsFilter = votingsFilter & votingsFilterBuilder.Regex(o => o.Name, queryParameters.Name);

            /*
            // Sorting
            //var votingsSorter = Builders<VoterVotingReference>.Sort.Ascending(o => o.Name);

            //Predicate<VoterVotingReference> votingsFilter = (o => o.VotingId == "sda");

            Comparison<VoterVotingReference> votingsSorter = ((a, b) => a.EndDate.CompareTo(b.EndDate));
            if (queryParameters.SortOrder == "Ascending")
            {
                votingsSorter = ((a, b) => a.EndDate.CompareTo(b.EndDate));
                if (queryParameters.SortBy != null)
                {

                }
            } 
            else if (queryParameters.SortOrder == "Descending") 
            {
                votingsSorter = ((a, b) => b.EndDate.CompareTo(a.EndDate));
                if (queryParameters.SortBy != null)
                {
                    string by = queryParameters.SortBy;

                    PropertyInfo prop = typeof(VoterVotingReference).GetProperty(queryParameters.SortBy);
                   // prop.GetValue(customer, null);


                    votingsSorter = ((a, b) => prop.GetValue(a, null).CompareTo(prop.GetValue(b, null)));
                }
            }
            */
            Comparison<VoterVotingReference> votingsSort = ((a, b) => a.VotingId.CompareTo(b.VotingId));

            var votings = voter.VotingReferences.AggregateByPage(votingsFilter, votingsSort, queryParameters.PageNumber, queryParameters.PageSize);

            var result = new PagedList<VoterVotingReference>(votings.data, votings.totalItemCount, queryParameters.PageNumber, queryParameters.PageSize);

            return result;
        }

        public async Task<Voter_BasicInfo_DTO> UpdateVoter(string voterId, Voter_Update_DTO voterUpdateData)
        {
            // TODO: Add transaction here {

            var voterFilterBuilder = Builders<Voter>.Filter;
            var voterFilter = voterFilterBuilder.Eq(o => o.Id, voterId); 

            var voterUpdateBuilder = Builders<Voter>.Update;
            var voterUpdate = voterUpdateBuilder
                .Set("FirstName", voterUpdateData.FirstName)
                .Set("LastName", voterUpdateData.LastName)
                .Set("Email", voterUpdateData.Email)
                .Set("Password", voterUpdateData.Password);

            var voterUpdateTask = votersCollection.UpdateOneAsync(voterFilter, voterUpdate);
            await Task.WhenAll(voterUpdateTask);
            var voterUpdateR = voterUpdateTask.Result;

            // TODO: Replace this with advanced query that will update and return updated (like in utils)
            var voterGetTask = votersCollection.Find(voterFilter).FirstOrDefaultAsync();
            await Task.WhenAll(voterGetTask);
            var voter = voterGetTask.Result;

            // Update data in registration requests
            for (int i = 0; i < voter.VotingReferences.Count; i++)
            {
                var votingReferenceFilter = Builders<RegistrationRequest>.Filter.Eq(o => o.Id, voter.VotingReferences[i].RegistrationRequestId);
                var registrationRequestUpdate = Builders<RegistrationRequest>.Update
                                .Set("VoterFirstName", voterUpdateData.FirstName)
                                .Set("VoterLastName", voterUpdateData.LastName)
                                ;
                var registrationRequestUpdateTask = GetCollection<RegistrationRequest>(registrationRequestsDatabase, "voting_" + voter.VotingReferences[i].VotingId).FindOneAndUpdateAsync(votingReferenceFilter, registrationRequestUpdate);
                await Task.WhenAll(registrationRequestUpdateTask); // TODO: Store and check tasks in list or bulk write?
            }
            // TODO: Add transaction here }

            // Create DTO
            var result = new Voter_BasicInfo_DTO()
            {
                Id = voter.Id,
                FirstName = voter.FirstName,
                LastName = voter.LastName,
                Email = voter.Email
            };

            return result;
        }

        public async Task<Voter_BasicInfo_DTO> AddVoter(Voter_Add_DTO voterAddData)
        {
            var newVoter = new Voter()
            {
                // Id
                FirstName = voterAddData.FirstName,
                LastName = voterAddData.LastName,
                Email = voterAddData.Email,
                Password = voterAddData.Password,
                RegistrationDate = DateTime.Now,
                VotingReferences = new List<VoterVotingReference>()
            };

            var voterAddTask = votersCollection.InsertOneAsync(newVoter);
            await Task.WhenAll(voterAddTask);

            var result = new Voter_BasicInfo_DTO()
            {
                Id = newVoter.Id,
                FirstName = newVoter.FirstName,
                LastName = newVoter.LastName,
                Email = newVoter.Email
            };

            return result;
        }
    }
}
