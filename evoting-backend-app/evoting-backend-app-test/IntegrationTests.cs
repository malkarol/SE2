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

namespace evoting_backend_app_test
{
    public class IntegrationTests
    {
        EVotingDatabaseSettings eVotingDatabaseSettings;

        [SetUp]
        public void Setup()
        {
            // Create database configuration
            string solutionDirectory = Directory.GetParent(Environment.CurrentDirectory).Parent.Parent.Parent.FullName;
            var configuration = new ConfigurationBuilder().AddJsonFile(solutionDirectory + @"\evoting-backend-app\appsettings.json").Build();
            eVotingDatabaseSettings = new EVotingDatabaseSettings();
            IConfigurationSection section = configuration.GetSection(nameof(EVotingDatabaseSettings));
            section.Bind(eVotingDatabaseSettings);
        }

        // Test if backend app connects to mongo database server
        [Test]
        public void DatabaseServerConnection()
        {
            // Arrange
            var client = new MongoClient(eVotingDatabaseSettings.ConnectionString);
            var mainDatabase = client.GetDatabase(eVotingDatabaseSettings.MainDatabaseName);

            // Assert
            Assert.NotNull(mainDatabase);
        }

        // Test if mongo database sever contains all necessary databases
        [Test]
        public void DatabaseServerDatabases()
        {
            // Arrange
            var client = new MongoClient(eVotingDatabaseSettings.ConnectionString);
            var mainDatabase = client.GetDatabase(eVotingDatabaseSettings.MainDatabaseName);
            var votesDatabaseName = client.GetDatabase(eVotingDatabaseSettings.VotesDatabaseName);

            // Assert
            Assert.NotNull(mainDatabase);
            Assert.NotNull(votesDatabaseName);
        }

        // Test if mainDatabase contains all necessary collections
        [Test]
        public void DatabaseCollections()
        {
            // Arrange
            var client = new MongoClient(eVotingDatabaseSettings.ConnectionString);
            var mainDatabase = client.GetDatabase(eVotingDatabaseSettings.MainDatabaseName);
           
            var voters = mainDatabase.GetCollection<Voter>(eVotingDatabaseSettings.VotersCollectionName);
            var coordinators = mainDatabase.GetCollection<Coordinator>(eVotingDatabaseSettings.CoordinatorsCollectionName);
            var votings = mainDatabase.GetCollection<Voting>(eVotingDatabaseSettings.VotingsCollectionName);
            var registrationRequests = mainDatabase.GetCollection<RegistrationRequest>(eVotingDatabaseSettings.RegistrationRequestsCollectionName);

            // Assert
            Assert.NotNull(voters);
            Assert.NotNull(coordinators);
            Assert.NotNull(votings);
            Assert.NotNull(registrationRequests);
        }

        [Test]
        public void ServiceDatabaseConnection()
        {
            // Arrange
            VotersService votersService = new VotersService(eVotingDatabaseSettings);

            // Act
            var result = votersService.GetAllVoters();

            // Assert
            Assert.NotNull(result);
        }

        [Test]
        public void VotersControllerGetServiceAndDatabasePipeline()
        {
            // Arrange
            VotersService votersService = new VotersService(eVotingDatabaseSettings);
            VotersController votersController = new VotersController(votersService);

            // Act
            var result = votersController.Get();

            // Assert
            Assert.NotNull(result);
        }
    }
}