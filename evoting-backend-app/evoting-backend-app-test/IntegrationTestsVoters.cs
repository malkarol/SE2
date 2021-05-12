using NUnit.Framework;
using evoting_backend_app.Services;
using evoting_backend_app;

using Microsoft.AspNetCore.Mvc.Testing;
using System.Net.Http;
using System.Net;
using System.Threading.Tasks;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Moq;
using System.Text.Json;
using System;
using System.IO;
using MongoDB.Driver;
using evoting_backend_app.Models;
using evoting_backend_app.Controllers;
using MongoDB.Bson.Serialization;
using MongoDB.Bson;
using Microsoft.AspNetCore.Mvc;
using static evoting_backend_app.Utils;

namespace evoting_backend_app_test
{
    public class IntegrationTestsVoters
    {
        EVotingDatabaseSettings eVotingDatabaseSettings;
        VotingsController votingsController;
        VotersController votersController;

        IMongoDatabase mainDatabase;
        IMongoDatabase votesDatabase;
        IMongoDatabase registrationRequestsDatabase;

        IMongoCollection<BsonDocument> votingsCollection;
        IMongoCollection<BsonDocument> votersCollection;

        [SetUp]
        public void Setup()
        {
            // Create database configuration
            string settingFilePath = Path.Combine(Path.GetDirectoryName(System.Reflection.Assembly.GetExecutingAssembly().Location), @"appsettings.json");
            var configuration = new ConfigurationBuilder().AddJsonFile(settingFilePath).Build();
            eVotingDatabaseSettings = new EVotingDatabaseSettings();
            IConfigurationSection section = configuration.GetSection(nameof(EVotingDatabaseSettings));
            eVotingDatabaseSettings.Test = true; // Use test connection string
            section.Bind(eVotingDatabaseSettings);

            // Instantiate services and controllers
            var votingsService = new VotingsService(eVotingDatabaseSettings);
            var votersService = new VotersService(eVotingDatabaseSettings);
            votingsController = new VotingsController(votingsService);
            votersController = new VotersController(votersService);

            var client = new MongoClient(eVotingDatabaseSettings.ConnectionString);

            // Get databases
            mainDatabase = client.GetDatabase(eVotingDatabaseSettings.MainDatabaseName);
            votesDatabase = client.GetDatabase(eVotingDatabaseSettings.VotesDatabaseName);
            registrationRequestsDatabase = client.GetDatabase(eVotingDatabaseSettings.RegistrationRequestsDatabaseName);

            // Get collections
            votingsCollection = mainDatabase.GetCollection<BsonDocument>(eVotingDatabaseSettings.VotingsCollectionName);
            votersCollection = mainDatabase.GetCollection<BsonDocument>(eVotingDatabaseSettings.VotersCollectionName);
        }

        [Test]
        public void GetVoter()
        {
            // Arrange
            var expectedFirstName = "TestFirstName";

            string voter = @" 
                {
	                ""_id"": { ""$oid"": ""606b498d982ca2c8da77e000"" },
	                ""FirstName"": ""TestFirstName"",
	                ""LastName"": ""TestLastName"",
	                ""Email"": ""TestEmail"",
	                ""Password"": ""test"",
	                ""RegistrationDate"": ""2021-06-04T13:05:48.392Z"",
	                ""VotingReferences"": []
                }
            ";

            Task.Run(async () => {

                // Act
                var document = BsonSerializer.Deserialize<BsonDocument>(voter);
                var voterAddTask = votersCollection.InsertOneAsync(document);
                await Task.WhenAll(voterAddTask);

                var voterGetTask = votersController.GetVoter("606b498d982ca2c8da77e000");
                await Task.WhenAll(voterGetTask);
                var actual = (voterGetTask.Result.Result as ObjectResult).Value as Voter_BasicInfo_DTO;

                // Assert
                Assert.AreEqual(expectedFirstName, actual.FirstName);

                // Cleanup
                var voterFilter = Builders<BsonDocument>.Filter.Eq("_id", new ObjectId("606b498d982ca2c8da77e000"));
                votersCollection.DeleteOne(voterFilter);

            }).GetAwaiter().GetResult();
        }

        [Test]
        public void AddVoter()
        {
            // Arrange
            var voterAddData = new Voter_Add_DTO()
            {
                FirstName = "NewTestFirstName",
                LastName = "NewTesLastName",
                Email = "NewTestEmail",
                Password = "NewTestPassword"
            };

            var expectedFirstName = "NewTestFirstName";

            Task.Run(async () => {

                // Act
                var voterAddTask = votersController.AddVoter(voterAddData);
                await Task.WhenAll(voterAddTask);
                var actual = (voterAddTask.Result.Result as ObjectResult).Value as Voter_BasicInfo_DTO;

                // Assert
                Assert.AreEqual(expectedFirstName, actual.FirstName);

                // Cleanup
                var voterFilter = Builders<BsonDocument>.Filter.Eq("_id", new ObjectId(actual.Id));
                votersCollection.DeleteOne(voterFilter);

            }).GetAwaiter().GetResult();
        }

        [Test]
        public void UpdateVoter()
        {
            // Arrange
            var expectedFirstName = "UpdatedTestFirstName";

            string voter = @" 
                {
	                ""_id"": { ""$oid"": ""606b498d982ca2c8da77e000"" },
	                ""FirstName"": ""TestFirstName"",
	                ""LastName"": ""TestLastName"",
	                ""Email"": ""TestEmail"",
	                ""Password"": ""test"",
	                ""RegistrationDate"": ""2021-06-04T13:05:48.392Z"",
	                ""VotingReferences"": []
                }
            ";

            var voterUpdateData = new Voter_Update_DTO() {
                FirstName = "UpdatedTestFirstName",
                LastName = "UpdatedTesLastName",
                Email = "UpdatedTestEmail",
                Password = "UpdatedTestPassword"
            };

            Task.Run(async () => {

                // Act
                var document = BsonSerializer.Deserialize<BsonDocument>(voter);
                votersCollection.InsertOne(document);

                var voterUpdateTask = votersController.UpdateVoter("606b498d982ca2c8da77e000", voterUpdateData);
                await Task.WhenAll(voterUpdateTask);
                var actual = (voterUpdateTask.Result.Result as ObjectResult).Value as Voter_BasicInfo_DTO;

                // Assert
                Assert.AreEqual(expectedFirstName, actual.FirstName);
                // TODO: Test also registration requests update

                // Cleanup
                var voterFilter = Builders<BsonDocument>.Filter.Eq("_id", new ObjectId("606b498d982ca2c8da77e000"));
                votersCollection.DeleteOne(voterFilter);

            }).GetAwaiter().GetResult();
        }

        [Test]
        public void GetVoterVotings()
        {
            // Arrange 
            string voter = @" 
                {
	                ""_id"": { ""$oid"": ""606b498d982ca2c8da77e000"" },
	                ""FirstName"": ""TestFirstName"",
	                ""LastName"": ""TestLastName"",
	                ""Email"": ""TestEmail"",
	                ""Password"": ""test"",
	                ""RegistrationDate"": ""2021-06-04T13:05:48.392Z"",
	                ""VotingReferences"": [
                        {
                            ""VotingId"": { ""$oid"": ""606b498d982ca2c8da77e001"" },
                            ""Name"": ""Best Pizza"",
			                ""StartDate"": ""2021-05-04T13:05:48.392Z"",
			                ""EndDate"": ""2021-05-04T14:05:48.392Z"",
			                ""RegistrationRequestId"": { ""$oid"": ""606b498d982ca2c8da77e101"" },
			                ""RegistrationStatus"": ""Accepted"",
			                ""RequestedDate"": ""2021-05-04T13:06:48.392Z"",
			                ""VoteId"": { ""$oid"": ""606b498d982ca2c8da77e201"" },
			                ""VoteDate"": ""2021-05-04T13:07:48.392Z""
                        },
                    ]
                }
            ";

            string voting = @" 
                {
                  ""_id"": { ""$oid"": ""606b498d982ca2c8da77e001"" },
                  ""Name"": ""Best Pizza"",
                  ""Description"": ""Vote for best pizza in this voting"",
                  ""StartDate"": ""2021-05-06T17:48:19.872Z"",
                  ""EndDate"": ""2021-05-10T17:48:19.872Z"",
                  ""Options"": [
                    {
                      ""Number"": 1,
                      ""Name"": ""Pinapple"",
                      ""Description"": ""Mmm"",
                      ""VoteCount"": 0
                    },
	                {
                      ""Number"": 2,
                      ""Name"": ""Not Pinapple"",
                      ""Description"": ""Ohh"",
                      ""VoteCount"": 0
                    }
                  ],
                  ""FreeRegistration"": true
                }
            ";

            string registrationRequest = @"
                {
                  ""_id"": { ""$oid"": ""606b498d982ca2c8da77e101"" },
                  ""VoterId"": { ""$oid"": ""606b498d982ca2c8da77e000"" },
                  ""VoterFirstName"": ""TestFirstName"",
	              ""VoterLastName"": ""TestLastName"",
                  ""VotingId"": { ""$oid"": ""606b498d982ca2c8da77e001"" },
                  ""Comment"": ""This is sample request"",
                  ""RequestedDate"": ""2021-05-04T13:06:48.392Z"",
                  ""ResolvedDate"": ""2021-05-05T16:04:00.392Z"",
                  ""RegistrationStatus"": ""Accepted""
                }
            ";

            string vote = @"
                {
                  ""_id"": { ""$oid"": ""606b498d982ca2c8da77e201"" },
                  ""VotingId"": { ""$oid"": ""606b498d982ca2c8da77e001"" },
                  ""VoterId"": { ""$oid"": ""606b498d982ca2c8da77e000"" },
                  ""VoteDate"": ""2021-05-04T16:07:48.392Z"",
                  ""VotingOptionNumbers"": [
                    1
                  ]
                }
            ";

            var expectedFirstVotingName = "Best Pizza";

            Task.Run(async () => {

                // Act
                var voterDocument = BsonSerializer.Deserialize<BsonDocument>(voter);
                var voterAddTask = votersCollection.InsertOneAsync(voterDocument);

                var votingDocument = BsonSerializer.Deserialize<BsonDocument>(voting);
                var votingAddTask = votingsCollection.InsertOneAsync(votingDocument);

                var registrationRequestDocument = BsonSerializer.Deserialize<BsonDocument>(registrationRequest);
                var registrationRequestAddTask = GetCollection<BsonDocument>(registrationRequestsDatabase, "voting_" + "606b498d982ca2c8da77e001").InsertOneAsync(registrationRequestDocument);

                var voteDocument = BsonSerializer.Deserialize<BsonDocument>(vote);
                var voteAddTask = GetCollection<BsonDocument>(votesDatabase, "voting_" + "606b498d982ca2c8da77e001").InsertOneAsync(voteDocument);

                await Task.WhenAll(voterAddTask, votingAddTask, registrationRequestAddTask, voteAddTask);


                var votingsGetTask = votersController.GetVoterVotings("606b498d982ca2c8da77e000", new VotersController.Voter_Voting_BasicInfo_QueryParameters());
                await Task.WhenAll(votingsGetTask);
                var actual = (votingsGetTask.Result.Result as ObjectResult).Value as PagedList<VoterVotingReference>;

                // Assert
                Assert.AreEqual(expectedFirstVotingName, actual.Items[0].Name);
                // TODO: Test more votings, filtering, paging and sorting

                // Cleanup
                var voterFilter = Builders<BsonDocument>.Filter.Eq("_id", new ObjectId("606b498d982ca2c8da77e000"));
                var voterDeleteTask = votersCollection.DeleteOneAsync(voterFilter);

                var votingFilter = Builders<BsonDocument>.Filter.Eq("_id", new ObjectId("606b498d982ca2c8da77e001"));
                var votingDeleteTask = votingsCollection.DeleteOneAsync(votingFilter);

                var votesCollectionTask = votesDatabase.DropCollectionAsync("voting_" + "606b498d982ca2c8da77e001");
                var registrationRequestsCollectionTask = registrationRequestsDatabase.DropCollectionAsync("voting_" + "606b498d982ca2c8da77e001");

                await Task.WhenAll(voterDeleteTask, votingDeleteTask, votesCollectionTask, registrationRequestsCollectionTask);

            }).GetAwaiter().GetResult();
        }

    }
}