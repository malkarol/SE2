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
            string settingFilePath = Path.Combine(Path.GetDirectoryName(System.Reflection.Assembly.GetExecutingAssembly().Location), @"appsettings.json");
            var configuration = new ConfigurationBuilder().AddJsonFile(settingFilePath).Build();
            eVotingDatabaseSettings = new EVotingDatabaseSettings();
            IConfigurationSection section = configuration.GetSection(nameof(EVotingDatabaseSettings));
            eVotingDatabaseSettings.Test = true; // Use test connection string
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
            var expected = "mongodb://root:root@host.docker.internal:27018";

            // Act
            var actual = eVotingDatabaseSettings.ConnectionString;

            // Assert
            Assert.AreEqual(expected, actual);
        }
    }
}