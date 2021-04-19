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

namespace evoting_backend_app_test
{
    public class Tests
    {

        [SetUp]
        public void Setup()
        {
        }

        [Test]
        public void Test1()
        {
            // https://blog.markvincze.com/overriding-configuration-in-asp-net-core-integration-tests/
            // https://www.youtube.com/watch?v=7roqteWLw4s
            // https://github.com/wswind/AspNetCore-UnitTest-Sample/blob/master/WebApiDemo.UnitTest/UnitTest1.cs
            //var appFactory = new WebApplicationFactory<Startup>();
            //var client = appFactory.CreateClient();
            //client.GetAsync(ApiRoutes.Posts.GetAll);
            //private Mock<VotersService> serviceMock = new Mock<VotersService>();

            // Create database configuration
            string solutionDirectory = Directory.GetParent(Environment.CurrentDirectory).Parent.Parent.Parent.FullName;
            var configuration = new ConfigurationBuilder().AddJsonFile(solutionDirectory + @"\evoting-backend-app\appsettings.json").Build();
            EVotingDatabaseSettings eVotingDatabaseSettings = new EVotingDatabaseSettings();
            IConfigurationSection section = configuration.GetSection(nameof(EVotingDatabaseSettings));
            section.Bind(eVotingDatabaseSettings);

            // Console.WriteLine(eVotingDatabaseSettings.ConnectionString);
            VotersService votersService = new VotersService(eVotingDatabaseSettings);


            Assert.Pass();
        }
    }
}