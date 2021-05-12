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
    /// <summary>
    /// Data for Name, StartDate, EndDate is shared/duplicated with respective Voting entry in database
    /// Data for RegistrationStatus, RequestedDate is shared/duplicated with respective RegistrationRequest entry in database
    /// Data for VoteDate is shared/duplicated with respective Vote entry in database
    /// 
    /// (Extended Reference Pattern)
    /// This means that if data in these entries is changed then it needs to be changed also in all VoterVotingReference entries
    /// 
    /// 
    /// 
    /// There is also one more approach possible:
    /// (Full Relational Pattern)
    /// Limit VoterVotingReference entry to VotingId, VoteId and RegistrationRequestId only
    /// However drawback of such approach is that when we would like to return data for Name, StartDate, EndDate, RegistrationStatus, RequestedDate and VoteDate
    /// we would need to create DTO and fill it with data fetched from respective Voting, RegistrationRequest and Vote entries
    /// Such fetch means 3 queries instead of 1 (in current, shared/duplicated data apporach) what for thousands requests in the same time may be very resource-consuming (20 times more)
    /// Moreover filtering and sorting in such approach may be much harder or even impossible to do on database side.
    /// 
    /// There is also subapproach, which is fetching (joining) on database side using aggregation (https://stackoverflow.com/questions/57405606/how-to-join-multiple-collections-using-mongodb-c-sharp-driver)
    /// Problem here is that we not processing entries, we are processing entries that are in list of other entry but such filtering should be possible even in this case.
    /// This is much better approach than doing it on backend side
    /// 
    /// ???
    /// (Here we are using list which is part of Voter entry, what means that filtering and sorting have to be done on backend 
    /// side but it is not that much problem because usually Voters don't have many VoterVotingReferences)
    /// 
    /// </summary>  

    public class VoterVotingReference
    {
        [BsonRepresentation(BsonType.ObjectId)]
        public string VotingId { get; set; }

        // Data shared with Voting
        public string Name { get; set; }

        // Data shared with Voting
        public DateTime StartDate { get; set; }

        // Data shared with Voting
        public DateTime EndDate { get; set; }

        [BsonRepresentation(BsonType.ObjectId)]
        public string RegistrationRequestId { get; set; }

        // Data shared with RegistrationRequest
        [JsonConverter(typeof(JsonStringEnumConverter))]
        [BsonRepresentation(BsonType.String)]
        public RegistrationRequestStatus RegistrationStatus { get; set; } // If voter is registered and can vote or still waiting for aproval of voting coordiantor

        // Data shared with RegistrationRequest
        public DateTime RequestedDate { get; set; }

        [BsonRepresentation(BsonType.ObjectId)]
        public string VoteId { get; set; } // Id of vote, null if vote has not been casted

        // Data shared with Vote
        public DateTime? VoteDate { get; set; }
    }

    public class Voter
    {
        [BsonId] [BsonRepresentation(BsonType.ObjectId)]
        public string Id { get; set; }

        public string FirstName { get; set; }

        public string LastName { get; set; }

        public string Email { get; set; }

        public string Password { get; set; }
     
        public DateTime RegistrationDate { get; set; }

        // User-specific
        public List<VoterVotingReference> VotingReferences { get; set; } // Votings in which voter can participate (or send registration request)

    }

    // --- Data Transfer Objects ---   

    public class Voter_BasicInfo_DTO // For [GET]/Voters/{id}
    {
        public string Id { get; set; }

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
        public String FirstName { get; set; }

        public String LastName { get; set; }

        public String Email { get; set; }

        public String Password { get; set; }
    }

    public class VoterVoting_BasicInfo_DTO // For [GET]/Voter/{id}/Votings (possibility of filtering)
    {
        public string VotingId { get; set; }

        public string Name { get; set; }

        public DateTime StartDate { get; set; }

        public DateTime EndDate { get; set; }

        public RegistrationRequestStatus RegistrationStatus { get; set; }

        public bool alreadyVoted { get; set; }
    }

    //public class Voter_Voting_BasicInfo_DTO // For [GET]/Voters/{id}/Votings
    //{
    //    // -- Fetched from VoterVotingReference --

    //    // - Fetched from Voting
    //    public string Id { get; set; }

    //    public string Name { get; set; }

    //    public DateTime StartDate { get; set; }

    //    public DateTime EndDate { get; set; }

    //    // - Fetched from RegistrationRequest -
    //    public RegistrationRequestStatus RegistrationStatus { get; set; } // If voter is registered and can vote or still waiting for aproval of voting coordiantor

    //    public DateTime RequestedDate { get; set; }

    //    // - Fetched from Vote -
    //    public DateTime VoteDate { get; set; }
    //}










}
