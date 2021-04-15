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
    public enum RegistrationRequestStatus
    {
        Pending,
        Resolved,
        Declined
    }

    public class RegistrationRequest
    {
        [BsonId]
        [BsonRepresentation(BsonType.ObjectId)]
        public string Id { get; set; }

        public string UserId { get; set; } // Id of voter user that sent the request
        public DateTime RequestedDate { get; set; }
        public string VotingId { get; set; } // Id of voting it refers to
        public string Comment { get; set; }

        [JsonConverter(typeof(JsonStringEnumConverter))]
        [BsonRepresentation(BsonType.String)]
        public RegistrationRequestStatus Status { get; set; }
    }
}
