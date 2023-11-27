using Microsoft.Azure.Cosmos;
using RobottoBackend.Models;

namespace RobottoBackend.Data.Repositories
{
    public class MissionRepository : IMissionRepository
    {
        private readonly Container _container;

        public MissionRepository(
            CosmosClient dbClient,
            string databaseName,
            string containerName)
        {
            _container = dbClient.GetContainer(databaseName, containerName);
        }
        
        public async Task AddItemAsync(Mission item)
        {
            await _container.CreateItemAsync<Mission>(item, new PartitionKey(item.Id));
        }

        public async Task DeleteItemAsync(string id)
        {
            await _container.DeleteItemAsync<Mission>(id, new PartitionKey(id));
        }

        public async Task<Mission> GetItemAsync(string id)
        {
            try
            {
                ItemResponse<Mission> response = await _container.ReadItemAsync<Mission>(id, new PartitionKey(id));
                return response.Resource;
            }
            catch(CosmosException ex) when (ex.StatusCode == System.Net.HttpStatusCode.NotFound)
            { 
                return null;
            }

        }

        public async Task<IEnumerable<Mission>> GetItemsAsync(string queryString)
        {
            var query = _container.GetItemQueryIterator<Mission>(new QueryDefinition(queryString));
            List<Mission> results = new List<Mission>();
            while (query.HasMoreResults)
            {
                var response = await query.ReadNextAsync();
                
                results.AddRange(response.ToList());
            }

            return results;
        }

        public async Task UpdateItemAsync(string id, Mission item)
        {
            await _container.UpsertItemAsync<Mission>(item, new PartitionKey(id));
        }
    }
}