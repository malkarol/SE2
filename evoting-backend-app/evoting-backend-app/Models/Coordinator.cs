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
    public enum CoordinatorType
    {
        Normal,
        Main
    }

    public class CoordinatorVotingReference
    {
        [BsonId] [BsonRepresentation(BsonType.ObjectId)]
        public string VotingId { get; set; }

        public string Comment { get; set; } // Additional task description
    }

    public class Coordinator
    {
        [BsonId] [BsonRepresentation(BsonType.ObjectId)]
        public string Id { get; set; }

        //User-specific
        [JsonConverter(typeof(JsonStringEnumConverter))]
        [BsonRepresentation(BsonType.String)]
        public CoordinatorType Type { get; set; }

        public string FirstName { get; set; }
      
        public string LastName { get; set; }
      
        public string Email { get; set; }
      
        public string Password { get; set; }

        //User-specific
        public List<CoordinatorVotingReference> VotingReferences { get; set; } // Votings which coordinator coordinate (supervise)
    }

    // --- Data Transfer Objects ---

    public class Coordinator_BasicInfo_DTO // For [GET]/Voters/{id}
    {
        [JsonConverter(typeof(JsonStringEnumConverter))]
        [BsonRepresentation(BsonType.String)]
        public CoordinatorType Type { get; set; }

        public string FirstName { get; set; }

        public string LastName { get; set; }

        public string Email { get; set; }
    }

    public class Coordinator_Add_DTO // For [PUT]/Voters/{id}
    {
        public CoordinatorType Type { get; set; }

        public string FirstName { get; set; }

        public string LastName { get; set; }

        public string Email { get; set; }

        public string Password { get; set; }
    }

    public class Coordinator_Update_DTO // For [PUT]/Voters/{id}
    {
        public string FirstName { get; set; }

        public string LastName { get; set; }

        public string Email { get; set; }

        public string Password { get; set; }
    }

    public class Coordinator_Voting_DTO // For [GET]/Coordinators/{id}/Votings (possibility of filtering)
    {
        // - Fetched from CoordinatorVotingReference -
        public string VotingId { get; set; }

        // - Fetched from Voting -
        public string VotingName { get; set; }

        public DateTime StartDate { get; set; }

        public DateTime EndDate { get; set; }

        public bool Active { get; set; }

    }



}
