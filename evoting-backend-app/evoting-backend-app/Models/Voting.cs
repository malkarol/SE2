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

    public class CoordinatorReference
    {
        [BsonRepresentation(BsonType.ObjectId)] 
        public string CoordinatorId { get; set; }

        public string CoordinatorFirstName { get; set; }

        public string CoordinatorLastName { get; set; }
    }

    public class Voting
    {
        [BsonId] [BsonRepresentation(BsonType.ObjectId)]
        public string Id { get; set; }

        public string Name { get; set; }

        public string Description { get; set; }

        public DateTime StartDate { get; set; }

        public DateTime EndDate { get; set; }    

        public List<VotingOption> Options { get; set;  }

        public bool FreeRegistration { get; set; } // If true then registration request is automatically approved and no coordinator action is required

        public VotingReport Report { get; set; }

        public List<CoordinatorReference> CoordinatorReferences { get; set; } // Coordinators coordinating/supervising the voting
    }

    // --- Data Transfer Objects ---

    public class Voting_BasicInfo_DTO // // For [GET]/Votings
    {
        public string Id { get; set; }

        public string Name { get; set; }

        public DateTime StartDate { get; set; }

        public DateTime EndDate { get; set; }
    }

    public class Voting_Add_DTO // // For [POST]/Votings
    {
        public string Id { get; set; }

        public string Name { get; set; }

        public string Description { get; set; }

        public DateTime StartDate { get; set; }

        public DateTime EndDate { get; set; }

        public List<VotingOption> Options { get; set; }

        public bool FreeRegistration { get; set; }
    }

    public class Voting_Update_DTO // For [PATCH]/Votings/{id}
    {
        public string Name { get; set; }

        public string Description { get; set; }

        public DateTime StartDate { get; set; }

        public DateTime EndDate { get; set; }

        public List<VotingOption> Options { get; set; }

        public bool FreeRegistration { get; set; } 
    }

    public class Voting_Voter_DTO // For [GET]/Votings/{id}/Voter/{id} // Personalized version of voting for voter, contains basic info about voting as well as information about vote casted if did
    {
        // - Fetched from Voting -
        public string Id { get; set; }

        public string Name { get; set; }

        public string Description { get; set; }

        public DateTime StartDate { get; set; }

        public DateTime EndDate { get; set; }

        public List<VotingOption> Options { get; set; }   

        // - Fetched from Vote -
        public DateTime? VoteDate { get; set; }

        public List<int> VotingOptionNumbers { get; set; }
    }

    public class Voting_Coordinator_DTO // For [GET]/Votings/{id}/Coordinator //  Personalized version of voting for coordinator, contains basic info about voting as well as data that should be visible only to coordinators
    {
        // - Fetched from Voting -
        public string Id { get; set; }

        public string Name { get; set; }

        public string Description { get; set; }

        public DateTime StartDate { get; set; }

        public DateTime EndDate { get; set; }

        public List<VotingOption> Options { get; set; }

        public bool FreeRegistration { get; set; } // If true then registration request is automatically approved and no coordinator action is required

        public VotingReport Report { get; set; }

        public List<CoordinatorReference> CoordinatorReferences { get; set; } // Coordinators coordinating/supervising the voting
    }

    public class Voting_AddCoordinator_DTO
    {
        public string CoordinatorId { get; set; }

        public string Comment { get; set; }
    }

}
