using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace evoting_backend_app
{
    public interface IEVotingDatabaseSettings
    {    
        public string Host { get; set; }
        public int Port { get; set; }
        public string User { get; set; }
        public string Password { get; set; }
        public string ConnectionString { get; }

        public string MainDatabaseName { get; set; }
        public string VotersCollectionName { get; set; }
        public string CoordinatorsCollectionName { get; set; }
        public string VotingsCollectionName { get; set; }
        
        public string VotesDatabaseName { get; set; }

        public string RegistrationRequestsDatabaseName { get; set; }
    }

    public class EVotingDatabaseSettings : IEVotingDatabaseSettings
    {
       
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

        public string MainDatabaseName { get; set; }
        public string VotersCollectionName { get; set; }
        public string CoordinatorsCollectionName { get; set; }
        public string VotingsCollectionName { get; set; }     
       
        public string VotesDatabaseName { get; set; }

        public string RegistrationRequestsDatabaseName { get; set; }
    }

}
