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

namespace evoting_backend_app.Models
{
    public enum RegistrationRequestDecision
    {
        Accept,
        Reject
    }


    public enum RegistrationRequestStatus
    {
        Pending,
        Accepted,
        Rejected
    }

    public class RegistrationRequest
    {
        [BsonId] [BsonRepresentation(BsonType.ObjectId)]
        public string Id { get; set; }

        [BsonId] [BsonRepresentation(BsonType.ObjectId)]
        public string VoterId { get; set; } // Id of voter user that sent the request
        
        public string VotingId { get; set; } // Id of voting it refers to
       
        public string Comment { get; set; }

        public DateTime RequestedDate { get; set; }

        [JsonConverter(typeof(JsonStringEnumConverter))]
        [BsonRepresentation(BsonType.String)]
        public RegistrationRequestStatus Status { get; set; }
    }

    // --- Data Transfer Objects ---

    public class RegistrationRequest_Decision_DTO // For [POST]/RegistrationRequests/{id}/Decide
    {
        public string CoordinatorId { get; set; } // Id of coordinator that makes the decision

        [JsonConverter(typeof(JsonStringEnumConverter))]
        [BsonRepresentation(BsonType.String)]
        public RegistrationRequestDecision Decision { get; set; }
    }

    public class RegistrationRequest_Add_DTO // For [POST]/Votings/{id}/Join
    {
        public string VoterId { get; set; } // Id of voter that wants to join

        public string VotingId { get; set; } // Id of voting to which voter wants to join

        public string Comment { get; set; }
    }

}
