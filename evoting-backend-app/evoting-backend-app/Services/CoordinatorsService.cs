using MongoDB.Driver;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using evoting_backend_app.Models;
using MongoDB.Bson;
using static evoting_backend_app.Controllers.CoordinatorsController;

namespace evoting_backend_app.Services
{
    public class CoordinatorsService
    {

        private readonly IMongoDatabase mainDatabase;
        private readonly IMongoDatabase votesDatabase;
        private readonly IMongoDatabase registrationRequestsDatabase;

        private readonly IMongoCollection<Coordinator> coordinatorsCollection;
        private readonly IMongoCollection<Voter> votersCollection;
        private readonly IMongoCollection<Voting> votingsCollection;


        public CoordinatorsService(IEVotingDatabaseSettings settings)
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

        public async Task<PagedList<Coordinator_BasicInfo_DTO>> GetAllCoordinators(Coordinator_BasicInfo_QueryParameters queryParameters)
        {
            // Filtering
            var coordinatorsFilterBuilder = Builders<Coordinator>.Filter;
            var coordinatorsFilter = coordinatorsFilterBuilder.Empty;
            if (queryParameters.Type != null)
                coordinatorsFilter = coordinatorsFilter & coordinatorsFilterBuilder.Eq(o => o.Type, queryParameters.Type);
            if (queryParameters.FirstName != null)
                coordinatorsFilter = coordinatorsFilter & coordinatorsFilterBuilder.Regex(o => o.FirstName, queryParameters.FirstName);
            if (queryParameters.LastName != null)
                coordinatorsFilter = coordinatorsFilter & coordinatorsFilterBuilder.Regex(o => o.LastName, queryParameters.LastName);
            if (queryParameters.Email != null)
                coordinatorsFilter = coordinatorsFilter & coordinatorsFilterBuilder.Regex(o => o.LastName, queryParameters.LastName);
           
            // Sorting
            var coordinatorsSort = Builders<Coordinator>.Sort.Ascending(o => o.FirstName);

            // Finding and paging
            var coordinatorsGetTask = coordinatorsCollection.AggregateByPage(coordinatorsFilter, coordinatorsSort, queryParameters.PageNumber, queryParameters.PageSize);
            await Task.WhenAll(coordinatorsGetTask);
            var coordinators = coordinatorsGetTask.Result;

            // TODO: Maybe some cleanup using projecting?
            var coordinatorsBasicInfo = coordinators.data.ConvertAll(o => new Coordinator_BasicInfo_DTO { Id = o.Id, Type = o.Type, FirstName = o.FirstName, LastName = o.LastName, Email = o.Email });

            var coordinatorsPage = new PagedList<Coordinator_BasicInfo_DTO>(coordinatorsBasicInfo, coordinators.totalItemCount, queryParameters.PageNumber, queryParameters.PageSize);

            return coordinatorsPage;
        }

        public async Task<Coordinator_BasicInfo_DTO> GetCoordinator(string coordinatorId)
        {
            var coordinatorFilter = Builders<Coordinator>.Filter.Eq(o => o.Id, coordinatorId);
            var coordinatorGetTask = coordinatorsCollection.Find(coordinatorFilter).FirstOrDefaultAsync();
            await Task.WhenAll(coordinatorGetTask);
            var coordinator = coordinatorGetTask.Result;

            var result = new Coordinator_BasicInfo_DTO()
            {
                Id = coordinator.Id,
                Type = coordinator.Type,
                FirstName = coordinator.FirstName,
                LastName = coordinator.LastName,
                Email = coordinator.Email
            };

            return result;
        }

        public async Task<Coordinator_BasicInfo_DTO> UpdateCoordinator(string coordinatorId, Coordinator_Update_DTO coordinatorUpdateData)
        {
            // TODO: Add transaction here {

            var coordinatorFilter = Builders<Coordinator>.Filter.Eq(o => o.Id, coordinatorId);

            var coordinatorUpdate = Builders<Coordinator>.Update
                .Set("FirstName", coordinatorUpdateData.FirstName)
                .Set("LastName", coordinatorUpdateData.LastName)
                .Set("Email", coordinatorUpdateData.Email)
                .Set("Password", coordinatorUpdateData.Password);

            var coordinatorUpdateTask = coordinatorsCollection.UpdateOneAsync(coordinatorFilter, coordinatorUpdate);
            await Task.WhenAll(coordinatorUpdateTask);
            var coordinatorUpdateR = coordinatorUpdateTask.Result;

            // TODO: Replace this with advanced query that will update and return updated (like in utils)
            var coordinatorGetTask = coordinatorsCollection.Find(coordinatorFilter).FirstOrDefaultAsync();
            await Task.WhenAll(coordinatorGetTask);
            var coordinator = coordinatorGetTask.Result;

            // Update data in coordinator references in all votings
            for (int i = 0; i < coordinator.VotingReferences.Count; i++)
            {
                var votingCoordinatorReferenceFilter = Builders<Voting>.Filter.Where(o => o.Id == coordinator.VotingReferences[i].VotingId && o.CoordinatorReferences.Any(c => c.CoordinatorId == coordinatorId));
                var votingCoordinatorReferenceUpdate = Builders<Voting>.Update
                                .Set(o => o.CoordinatorReferences[-1].CoordinatorFirstName, coordinatorUpdateData.FirstName)
                                .Set(o => o.CoordinatorReferences[-1].CoordinatorLastName, coordinatorUpdateData.LastName)
                                ;
                var coordinatorReferenceUpdateTask = votingsCollection.FindOneAndUpdateAsync(votingCoordinatorReferenceFilter, votingCoordinatorReferenceUpdate);
                await Task.WhenAll(coordinatorReferenceUpdateTask); // TODO: Store and check tasks in list or bulk write?
            }
            // TODO: Add transaction here }

            // Create DTO
            var result = new Coordinator_BasicInfo_DTO()
            {
                Id = coordinator.Id,
                Type = coordinator.Type,
                FirstName = coordinator.FirstName,
                LastName = coordinator.LastName,
                Email = coordinator.Email
            };

            return result;
        }

        public async Task<Coordinator_BasicInfo_DTO> AddCoordinator(Coordinator_Add_DTO coordinatorAddData)
        {
            var newCoordinator = new Coordinator()
            {
                // Id
                Type = coordinatorAddData.Type,
                FirstName = coordinatorAddData.FirstName,
                LastName = coordinatorAddData.LastName,
                Email = coordinatorAddData.Email,
                Password = coordinatorAddData.Password,
                RegistrationDate = DateTime.Now,
                VotingReferences = new List<CoordinatorVotingReference>()
            };

            var coordinatorAddTask = coordinatorsCollection.InsertOneAsync(newCoordinator);
            await Task.WhenAll(coordinatorAddTask);

            var result = new Coordinator_BasicInfo_DTO()
            {
                Id = newCoordinator.Id,
                Type = newCoordinator.Type,
                FirstName = newCoordinator.FirstName,
                LastName = newCoordinator.LastName,
                Email = newCoordinator.Email
            };

            return result;
        }

        public async Task<PagedList<CoordinatorVoting_BasicInfo_DTO>> GetCoordinatorVotings(string coordinatorId, CoordinatorVoting_BasicInfo_QueryParameters queryParameters)
        {
            // Find coordinator
            var coordinatorFilter = Builders<Coordinator>.Filter.Eq(o => o.Id, coordinatorId);
            var getCoordinatorTask = coordinatorsCollection.Find(coordinatorFilter).FirstOrDefaultAsync();
            await Task.WhenAll(getCoordinatorTask);
            var coordinator = getCoordinatorTask.Result;

            // Filtering   
            Predicate<CoordinatorVotingReference> votingsFilter = (o => o != null);
            if (queryParameters.VotingName != null)
                votingsFilter = votingsFilter + (o => o.Name == queryParameters.VotingName);
            if (queryParameters.StartDate != null)
                votingsFilter = votingsFilter + (o => o.StartDate >= queryParameters.StartDate);
            if (queryParameters.EndDate != null)
                votingsFilter = votingsFilter + (o => o.EndDate <= queryParameters.EndDate);
            if (queryParameters.Active != null)
                votingsFilter = votingsFilter + (o => DateTime.Now >= o.StartDate && DateTime.Now <= o.EndDate);

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
            Comparison<CoordinatorVotingReference> votingsSort = ((a, b) => a.VotingId.CompareTo(b.VotingId));

            var votings = coordinator.VotingReferences.AggregateByPage(votingsFilter, votingsSort, queryParameters.PageNumber, queryParameters.PageSize);

            // TODO: Maybe some cleanup using projecting?
            var coordinatorVotingsBasicInfo = votings.data.ConvertAll(o => new CoordinatorVoting_BasicInfo_DTO { VotingId = o.VotingId, VotingName = o.Name, StartDate = o.StartDate, EndDate = o.EndDate });

            var coordinatorVotingsPage = new PagedList<CoordinatorVoting_BasicInfo_DTO>(coordinatorVotingsBasicInfo, votings.totalItemCount, queryParameters.PageNumber, queryParameters.PageSize);

            return coordinatorVotingsPage;
        }

    }
}
