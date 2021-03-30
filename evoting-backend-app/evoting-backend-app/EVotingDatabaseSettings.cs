using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace evoting_backend_app.Models
{
    public interface IEVotingDatabaseSettings
    {
        string UsersCollectionName { get; set; }
        string ConnectionString { get; set; }
        string DatabaseName { get; set; }
    }

    public class EVotingDatabaseSettings : IEVotingDatabaseSettings
    {
        public string UsersCollectionName { get; set; }
        public string ConnectionString { get; set; }
        public string DatabaseName { get; set; }
    }

    //public interface IEVotingDatabaseSettings
    //{
    //    public string UsersCollectionName { get; set; }
    //    public string Database { get; set; }
    //    public string Host { get; set; }
    //    public int Port { get; set; }
    //    public string User { get; set; }
    //    public string Password { get; set; }
    //    public string ConnectionString { get; }
    //}

    //public class EVotingDatabaseSettings : IEVotingDatabaseSettings
    //{
    //    public string UsersCollectionName { get; set; }
    //    public string Database { get; set; }
    //    public string Host { get; set; }
    //    public int Port { get; set; }
    //    public string User { get; set; }
    //    public string Password { get; set; }
    //    public string ConnectionString
    //    {
    //        get
    //        {
    //            if (string.IsNullOrEmpty(User) || string.IsNullOrEmpty(Password))
    //            {
    //                return $@"mongodb://{Host}:{Port}";
    //            }

    //            return $@"mongodb://{User}:{Password}@{Host}:{Port}";
    //        }
    //    }
    //}

}
