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
        Accepted,
        Rejected,
        Pending
    }

    public class RegistrationRequest
    {
        [BsonId] [BsonRepresentation(BsonType.ObjectId)]
        public string Id { get; set; }

        [BsonRepresentation(BsonType.ObjectId)]
        public string VoterId { get; set; } // Id of voter user that sent the request

        // Data shared with Voter
        public string VoterFirstName { get; set; }

        // Data shared with Voter
        public string VoterLastName { get; set; }

        [BsonRepresentation(BsonType.ObjectId)]
        public string VotingId { get; set; } // Id of voting it refers to
       
        public string Comment { get; set; }

        public DateTime RequestedDate { get; set; }

        // - Decision -

        public DateTime? ResolvedDate { get; set; }

        [JsonConverter(typeof(JsonStringEnumConverter))]
        [BsonRepresentation(BsonType.String)]
        public RegistrationRequestStatus Status { get; set; }

        [BsonRepresentation(BsonType.ObjectId)]
        public string CoordinatorId { get; set; }
    }

    // --- Data Transfer Objects ---

    public class RegistrationRequest_BasicInfo_DTO // For [GET]/Votings/{id}/RegistrationRequests
    {
        public string Id { get; set; }

        public DateTime RequestedDate { get; set; }

        public DateTime? ResolvedDate { get; set; }

        [JsonConverter(typeof(JsonStringEnumConverter))]
        [BsonRepresentation(BsonType.String)]
        public RegistrationRequestStatus Status { get; set; }
    }

    public class RegistrationRequest_Decision_DTO // For [POST]/RegistrationRequests/{id}/Decide
    {
        public string VotingId { get; set; }



        public string CoordinatorId { get; set; } // Id of coordinator that makes the decision

        [JsonConverter(typeof(JsonStringEnumConverter))]
        [BsonRepresentation(BsonType.String)]
        public RegistrationRequestDecision Decision { get; set; }
    }

    public class RegistrationRequest_Add_DTO // For [POST]/Votings/{id}/Join
    {
        public string VoterId { get; set; } // Id of voter that wants to join

        public string Comment { get; set; }
    }

}
