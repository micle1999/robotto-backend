using Microsoft.Azure.Cosmos;
using RobottoBackend.Models;

namespace RobottoBackend.Data.Repositories
{
    public class ResourceRepository : IResourceRepository
    {
        private readonly Container _container;

        public ResourceRepository(
            CosmosClient dbClient,
            string databaseName,
            string containerName)
        {
            _container = dbClient.GetContainer(databaseName, containerName);
        }
        
        public async Task AddItemAsync(Resource item)
        {
            await _container.CreateItemAsync<Resource>(item, new PartitionKey(item.Id));
        }

        public async Task DeleteItemAsync(string id)
        {
            await _container.DeleteItemAsync<Resource>(id, new PartitionKey(id));
        }

        public async Task<Resource> GetItemAsync(string id)
        {
            try
            {
                ItemResponse<Resource> response = await _container.ReadItemAsync<Resource>(id, new PartitionKey(id));
                return response.Resource;
            }
            catch(CosmosException ex) when (ex.StatusCode == System.Net.HttpStatusCode.NotFound)
            { 
                return null;
            }

        }

        public async Task<IEnumerable<Resource>> GetItemsAsync(string queryString)
        {
            var query = _container.GetItemQueryIterator<Resource>(new QueryDefinition(queryString));
            List<Resource> results = new List<Resource>();
            while (query.HasMoreResults)
            {
                var response = await query.ReadNextAsync();
                
                results.AddRange(response.ToList());
            }

            return results;
        }

        public async Task UpdateItemAsync(string id, Resource item)
        {
            await _container.UpsertItemAsync<Resource>(item, new PartitionKey(id));
        }
    }
}