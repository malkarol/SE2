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
    public class VoterVotingReference
    {
        [BsonId] [BsonRepresentation(BsonType.ObjectId)]
        public string VotingId { get; set; }

        [BsonId] [BsonRepresentation(BsonType.ObjectId)]
        public string RegistrationRequestId { get; set; }

        [BsonId] [BsonRepresentation(BsonType.ObjectId)]
        public string VoteId { get; set; } // Id of vote, null if vote has not been casted
    }

    public class Voter
    {
        [BsonId] [BsonRepresentation(BsonType.ObjectId)]
        public string Id { get; set; }

        public string FirstName { get; set; }

        public string LastName { get; set; }

        public string Email { get; set; }

        public string Password { get; set; }

        // User-specific
        public DateTime RegistrationDate { get; set; }

        public List<VoterVotingReference> VotingReferences { get; set; } // Votings in which voter can participate (or send registration request)
    }

    // --- Data Transfer Objects ---   

    public class Voter_BasicInfo_DTO // For [GET]/Voters/{id}
    {
        public string FirstName { get; set; }

        public string LastName { get; set; }

        public string Email { get; set; }
    }

    public class Voter_Add_DTO // For [POST]/Voters
    {
        public string FirstName { get; set; }

        public string LastName { get; set; }

        public string Email { get; set; }

        public string Password { get; set; }
    }

    public class Voter_Update_DTO // For [PUT]/Voters/{id}
    {
        public string FirstName { get; set; }

        public string LastName { get; set; }

        public string Email { get; set; }

        public string Password { get; set; }
    }

    public class Voter_Voting_BasicInfo_DTO // For [GET]/Voters/{id}/Votings
    {
        // - Fetched from VoterVotingReference -
        public string VotingId { get; set; }

        public string VotingName { get; set; }

        public DateTime StartDate { get; set; }

        public DateTime EndDate { get; set; }

        public bool Active { get; set; }

        // - Fetched from RegistrationRequest -
        public RegistrationRequestStatus RegistrationStatus { get; set; } // If voter is registered and can vote or still waiting for aproval of voting coordiantor

        public DateTime RequestedDate { get; set; }

        // - Fetched from Vote -
        public DateTime VoteDate { get; set; }

        public List<int> VotingOptionNumbers { get; set; }
    }





}
