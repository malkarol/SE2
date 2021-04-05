using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace evoting_backend_app.Models
{
    public interface IEVotingDatabaseSettings
    {
        public string DatabaseName { get; set; }
        public string Host { get; set; }
        public int Port { get; set; }
        public string User { get; set; }
        public string Password { get; set; }
        public string ConnectionString { get; }
        public string UsersCollectionName { get; set; }
        public string VotingsCollectionName { get; set; }
        public string RegistrationRequestsCollectionName { get; set; }
        public string CoordinatorsCollectionName { get; set; }

    }

    public class EVotingDatabaseSettings : IEVotingDatabaseSettings
    {
        public string DatabaseName { get; set; }
        public string Host { get; set; }
        public int Port { get; set; }
        public string User { get; set; }
        public string Password { get; set; }
        public string ConnectionString
        {
            get
            {
                if (string.IsNullOrEmpty(User) || string.IsNullOrEmpty(Password))
                {
                    return $@"mongodb://{Host}:{Port}";
                }

                return $@"mongodb://{User}:{Password}@{Host}:{Port}";
            }
        }
        public string UsersCollectionName { get; set; }
        public string VotingsCollectionName { get; set; }
        public string RegistrationRequestsCollectionName { get; set; }
        public string CoordinatorsCollectionName { get; set; }
    }

}
