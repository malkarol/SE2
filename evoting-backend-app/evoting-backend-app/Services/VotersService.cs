using MongoDB.Driver;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using evoting_backend_app.Models;
using MongoDB.Bson;

namespace evoting_backend_app.Services
{
    public class VotersService
    {
        private readonly IMongoCollection<Voter> voters;

        public VotersService(IEVotingDatabaseSettings settings)
        {
            var client = new MongoClient(settings.ConnectionString);
            var mainDatabase = client.GetDatabase(settings.MainDatabaseName);

            voters = mainDatabase.GetCollection<Voter>(settings.VotersCollectionName);
        }

      
    }
}
