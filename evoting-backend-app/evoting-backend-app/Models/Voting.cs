using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.Json.Serialization;
using System.Threading.Tasks;

namespace evoting_backend_app.Models
{
    public class VotingReport
    {
        public VotingOption Winner { get; set; }
        public int ParticipantCount { get; set; }
        // Some other stats that can be calculated after voting has finished
    }

    public class VotingOption
    {
        public int Number { get; set; }
        public string Name { get; set; }
        public string Description { get; set; }
        public int VoteCount { get; set; }
    }

    public class Voting
    {
        [BsonId]
        [BsonRepresentation(BsonType.ObjectId)]
        public string Id { get; set; }

        public string Name { get; set; }
        public string Description { get; set; }
        public DateTime StartDate { get; set; }
        public DateTime EndDate { get; set; }    
        public bool Active { get; set; }
        public List<VotingOption> Options { get; set; }
        public VotingReport Report { get; set; }
    }
}
