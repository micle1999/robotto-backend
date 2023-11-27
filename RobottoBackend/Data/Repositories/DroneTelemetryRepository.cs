using Microsoft.Azure.Cosmos;
using RobottoBackend.Models;

namespace RobottoBackend.Data.Repositories
{
    public class DroneTelemetryRepository : IDroneTelemetryRepository
    {
        private readonly Container _container;

        public DroneTelemetryRepository(
            CosmosClient dbClient,
            string databaseName,
            string containerName)
        {
            _container = dbClient.GetContainer(databaseName, containerName);
        }
        
        public async Task AddItemAsync(DroneTelemetry item)
        {
            await _container.CreateItemAsync<DroneTelemetry>(item, new PartitionKey(item.Id));
        }

        public async Task DeleteItemAsync(string id)
        {
            await _container.DeleteItemAsync<DroneTelemetry>(id, new PartitionKey(id));
        }

        public async Task<DroneTelemetry> GetItemAsync(string id)
        {
            try
            {
                ItemResponse<DroneTelemetry> response = await _container.ReadItemAsync<DroneTelemetry>(id, new PartitionKey(id));
                return response.Resource;
            }
            catch(CosmosException ex) when (ex.StatusCode == System.Net.HttpStatusCode.NotFound)
            { 
                return null;
            }

        }

        public async Task<IEnumerable<DroneTelemetry>> GetItemsAsync(string queryString)
        {
            var query = _container.GetItemQueryIterator<DroneTelemetry>(new QueryDefinition(queryString));
            List<DroneTelemetry> results = new List<DroneTelemetry>();
            while (query.HasMoreResults)
            {
                var response = await query.ReadNextAsync();
                
                results.AddRange(response.ToList());
            }

            return results;
        }

        public async Task UpdateItemAsync(string id, DroneTelemetry item)
        {
            await _container.UpsertItemAsync<DroneTelemetry>(item, new PartitionKey(id));
        }
    }
}