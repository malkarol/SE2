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
using evoting_backend_app.Controllers;
using System.Collections.Generic;
using evoting_backend_app.Models;
using Microsoft.AspNetCore.Mvc;

namespace evoting_backend_app_test
{
    public class UnitTests
    {
        // https://blog.markvincze.com/overriding-configuration-in-asp-net-core-integration-tests/
        // https://www.youtube.com/watch?v=7roqteWLw4s
        // https://github.com/wswind/AspNetCore-UnitTest-Sample/blob/master/WebApiDemo.UnitTest/UnitTest1.cs
        //var appFactory = new WebApplicationFactory<Startup>();
        //var client = appFactory.CreateClient();
        //client.GetAsync(ApiRoutes.Posts.GetAll);
        //private Mock<VotersService> serviceMock = new Mock<VotersService>();


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

        [Test]
        public void SettingsReadingBasic()
        {
            // Arrange
            var expected = "evotingMain";

            // Act
            var actual = eVotingDatabaseSettings.MainDatabaseName;

            // Assert
            Assert.AreEqual(expected, actual);
        }

        [Test]
        public void SettingsReadingConnectionString()
        {
            // Arrange       
            var expected = "mongodb://root:root@host.docker.internal:27017";

            // Act
            var actual = eVotingDatabaseSettings.ConnectionString;

            // Assert
            Assert.AreEqual(expected, actual);
        }

        /*
        // Does this test make sense?
        [Test]
        public void VotersControllerGet()
        {
            // Arrange
            Mock<VotersService> votersServiceMock = new Mock<VotersService>(eVotingDatabaseSettings);
            var result = new Task<IEnumerable<Voter>>(() => { return new List<Voter>(); });
            votersServiceMock.Setup(s => s.GetAllVoters()).Returns(result); // Mocked method has to be virtual or class has to be an interface
            VotersController votersController = new VotersController(votersServiceMock.Object);

            // Act
            var expected = new Task<ActionResult<IEnumerable<Voter>>>(null);
            var actual = votersController.Get();

            Console.WriteLine(result.GetType().ToString());
            Console.WriteLine(votersServiceMock.Object.GetAllVoters().GetType().ToString());
            Console.WriteLine(votersController.Get());
            Console.WriteLine(expected.GetType().ToString());
            // Assert
            Assert.AreEqual(expected, actual);
        }
        */

    }
}