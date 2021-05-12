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
using Newtonsoft.Json;

namespace evoting_backend_app_test
{
    public class IntegrationTests
    {
        EVotingDatabaseSettings eVotingDatabaseSettings;

        [SetUp]
        public void Setup()
        {
            // Create database configuration
            string settingFilePath = Path.Combine(Path.GetDirectoryName(System.Reflection.Assembly.GetExecutingAssembly().Location), @"appsettings.json");
            var configuration = new ConfigurationBuilder().AddJsonFile(settingFilePath).Build();
            eVotingDatabaseSettings = new EVotingDatabaseSettings();
            IConfigurationSection section = configuration.GetSection(nameof(EVotingDatabaseSettings));
            //eVotingDatabaseSettings.Test = true; // Use test connection string
            section.Bind(eVotingDatabaseSettings);
        }

        // Test if backend app connects to mongo database server
        [Test, Category("A")]
        public void DatabaseServerConnection()
        {
            // Arrange
            var client = new MongoClient(eVotingDatabaseSettings.ConnectionString);
            var mainDatabase = client.GetDatabase(eVotingDatabaseSettings.MainDatabaseName);



            // Assert
            Assert.NotNull(mainDatabase);
        }

        [Test, Category("A")]
        public void TestDatabaseServerConnection()
        {
            // Arrange
            var client = new MongoClient(eVotingDatabaseSettings.ConnectionString.Substring(0, eVotingDatabaseSettings.ConnectionString.Length - 1) + "8");
            var mainDatabase = client.GetDatabase(eVotingDatabaseSettings.MainDatabaseName);

            // Assert
            Assert.NotNull(mainDatabase);
        }

        // Test if mongo database sever contains all necessary databases
        [Test, Category("A")]
        public void DatabaseServerDatabases()
        {
            // Arrange
            var client = new MongoClient(eVotingDatabaseSettings.ConnectionString);
            var mainDatabase = client.GetDatabase(eVotingDatabaseSettings.MainDatabaseName);
            var votesDatabase = client.GetDatabase(eVotingDatabaseSettings.VotesDatabaseName);
            var registrationRequestDatabase = client.GetDatabase(eVotingDatabaseSettings.RegistrationRequestsDatabaseName);

            // Assert
            Assert.NotNull(mainDatabase);
            Assert.NotNull(votesDatabase);
            Assert.NotNull(registrationRequestDatabase);
        }

        // Test if mainDatabase contains all necessary collections
        [Test, Category("A")]
        public void MainDatabaseCollections()
        {
            // Arrange
            var client = new MongoClient(eVotingDatabaseSettings.ConnectionString);
            var mainDatabase = client.GetDatabase(eVotingDatabaseSettings.MainDatabaseName);

            var voters = mainDatabase.GetCollection<Voter>(eVotingDatabaseSettings.VotersCollectionName);
            var coordinators = mainDatabase.GetCollection<Coordinator>(eVotingDatabaseSettings.CoordinatorsCollectionName);
            var votings = mainDatabase.GetCollection<Voting>(eVotingDatabaseSettings.VotingsCollectionName);

            // Assert
            Assert.NotNull(voters);
            Assert.NotNull(coordinators);
            Assert.NotNull(votings);
        }
    }
}