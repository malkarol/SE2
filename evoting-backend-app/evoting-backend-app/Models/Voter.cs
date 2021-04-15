using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using MongoDB.Driver;
using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;
using MongoDB.Bson.IO;
using System.Text.Json;
using System.Text.Json.Serialization;
using Newtonsoft.Json.Converters;
using System.ComponentModel.DataAnnotations;
using System.Runtime.Serialization;

namespace evoting_backend_app.Models
{
    public class VoteReference
    {
        public string votingId { get; set; }
        public string voteId { get; set; }
    }

    public class Voter
    {
        [BsonId]
        [BsonRepresentation(BsonType.ObjectId)]
        public string Id { get; set; }

        public string FirstName { get; set; }
        public string LastName { get; set; }
        public string Email { get; set; }
        public string Password { get; set; }

        // User-specific
        public DateTime RegistrationDate { get; set; }
        public List<string> EligibleVotingsIds { get; set; } // Votings in which voter can participate
        public List<VoteReference> VoteReferences { get; set; } // References to votes that this voter cast
    }
}
