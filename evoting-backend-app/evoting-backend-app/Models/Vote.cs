using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.Json.Serialization;
using System.Threading.Tasks;

namespace evoting_backend_app.Models
{
    public class Vote
    {
        [BsonId]
        [BsonRepresentation(BsonType.ObjectId)]
        public string Id { get; set; }

        public string VotingId { get; set; }
        public string VoterId { get; set; }
        public DateTime VoteDate { get; set; }
        public int VotingOptionNumber { get; set; }
    }
}
