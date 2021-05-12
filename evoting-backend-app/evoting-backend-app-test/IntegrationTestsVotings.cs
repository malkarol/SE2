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
    public class IntegrationTestsVotings
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
        public void GetVotingVoter()
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
                  ""votingId"": { ""$oid"": ""606b498d982ca2c8da77e001"" },
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

            var expectedVotingName = "Best Pizza";
            var expectedVotingOptionNumber = 1;


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


                var votingGetTask = votingsController.GetVotingVoter("606b498d982ca2c8da77e001", "606b498d982ca2c8da77e000");
                await Task.WhenAll(votingGetTask);
                var actual = (votingGetTask.Result.Result as ObjectResult).Value as Voting_Voter_DTO;

                // Assert
                Assert.AreEqual(expectedVotingName, actual.Name);
                Assert.AreEqual(expectedVotingOptionNumber, actual.VotingOptionNumbers[0]);

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