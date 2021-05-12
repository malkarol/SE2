using evoting_backend_app.Models;
using MongoDB.Driver;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace evoting_backend_app
{
    public static class Utils
    {
        //public static IMongoCollection<Vote> GetVotesCollection(IMongoDatabase votesDatabase, string votingId)
        //{
        //    return votesDatabase.GetCollection<Vote>("voting_" + votingId);
        //}

        //public static IMongoCollection<RegistrationRequest> GetRegistrationRequestsCollection(IMongoDatabase registrationRequestsDatabase, string votingId)
        //{
        //    return registrationRequestsDatabase.GetCollection<RegistrationRequest>("voting_" + votingId);
        //}

        public static IMongoCollection<T> GetCollection<T>(IMongoDatabase database, string name) // Returns collection from different database
        {
            return database.GetCollection<T>(name);
        }

    }

    

    public class QueryParameters
    {
        const int maxPageSize = 10;
        public int PageNumber { get; set; } = 1;
        private int pageSize = 10;
        public int PageSize { get { return pageSize; } set { pageSize = Math.Clamp(value, 0, maxPageSize); } }

        public string SortOrder { get; set; } = "Descending";
    }

    public class PagedList<T>
    {
        public List<T> Items { get; private set; }
        public int CurrentPage { get; private set; }
        public int TotalPages { get; private set; }
        public int PageSize { get; private set; }
        public int TotalItemCount { get; private set; }
        public bool HasPrevious => CurrentPage > 1;
        public bool HasNext => CurrentPage < TotalPages;
        public PagedList(List<T> items, int totalItemCount, int pageNumber, int pageSize)
        {
            this.Items = items;
            TotalItemCount = totalItemCount;
            PageSize = pageSize;
            CurrentPage = pageNumber;
            TotalPages = (int)Math.Ceiling((double)totalItemCount / (double)pageSize);
        }
    }

    public class Id_DTO {

        public string Id { get; set; }

        public Id_DTO(string id)
        {
            this.Id = id;
        }
    }

    public static class MongoCollectionQueryByPageExtensions
    {
        public static async Task<(List<TDocument> data, int totalItemCount)> AggregateByPage<TDocument>(
            this IMongoCollection<TDocument> collection,
            FilterDefinition<TDocument> filterDefinition,
            SortDefinition<TDocument> sortDefinition,
            int pageNumber,
            int pageSize)
        {
            var countFacet = AggregateFacet.Create("totalItemCount",
                PipelineDefinition<TDocument, AggregateCountResult>.Create(new[]
                {
                PipelineStageDefinitionBuilder.Count<TDocument>()
                }));

            var dataFacet = AggregateFacet.Create("data",
                PipelineDefinition<TDocument, TDocument>.Create(new[]
                {
                PipelineStageDefinitionBuilder.Sort(sortDefinition),
                PipelineStageDefinitionBuilder.Skip<TDocument>((pageNumber - 1) * pageSize),
                PipelineStageDefinitionBuilder.Limit<TDocument>(pageSize),
                }));

            var aggregation = await collection.Aggregate()
                .Match(filterDefinition)
                .Facet(countFacet, dataFacet)
                .ToListAsync();

            var totalItemCount = aggregation.First()
                .Facets.First(x => x.Name == "totalItemCount")
                .Output<AggregateCountResult>()
                ?.FirstOrDefault()
                ?.Count;

            var data = aggregation.First()
                .Facets.First(x => x.Name == "data")
                .Output<TDocument>();

            return (new List<TDocument>(data), (int)totalItemCount);
        }
    }

    public static class ListQueryByPageExtensions
    {
        public static (List<T> data, int totalItemCount) AggregateByPage<T>(
            this List<T> collection,
            Predicate<T> filterDefinition,
            Comparison<T> sortDefinition,
            int pageNumber,
            int pageSize)
        {
            var filteredData = collection.FindAll(filterDefinition); // This may be much slower in case of searching for index (no hashing)
            filteredData.Sort(sortDefinition);

            var pagedData = filteredData.Skip((pageNumber - 1) * pageSize).Take(pageSize).ToList();
            var totalItemCount = collection.Count();

            return (new List<T>(pagedData), totalItemCount);
        }
    }
    public static class PredicateExtensions
    {
        public static Predicate<T> And<T>(Predicate<T> p1, Predicate<T> p2)
        {
            return x => p1(x) && p2(x);
        }
    }
}
