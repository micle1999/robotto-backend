using Microsoft.Azure.Cosmos;
using RobottoBackend.Models;

namespace RobottoBackend.Data.Repositories
{
    public class NaturalHazardRepository : INaturalHazardRepository
    {
        private readonly Container _container;

        public NaturalHazardRepository(
            CosmosClient dbClient,
            string databaseName,
            string containerName)
        {
            _container = dbClient.GetContainer(databaseName, containerName);
        }
        
        public async Task AddItemAsync(NaturalHazard item)
        {
            await _container.CreateItemAsync<NaturalHazard>(item, new PartitionKey(item.Id));
        }

        public async Task DeleteItemAsync(string id)
        {
            await _container.DeleteItemAsync<NaturalHazard>(id, new PartitionKey(id));
        }

        public async Task<NaturalHazard> GetItemAsync(string id)
        {
            try
            {
                ItemResponse<NaturalHazard> response = await _container.ReadItemAsync<NaturalHazard>(id, new PartitionKey(id));
                return response.Resource;
            }
            catch(CosmosException ex) when (ex.StatusCode == System.Net.HttpStatusCode.NotFound)
            { 
                return null;
            }

        }

        public async Task<IEnumerable<NaturalHazard>> GetItemsAsync(string queryString)
        {
            var query = _container.GetItemQueryIterator<NaturalHazard>(new QueryDefinition(queryString));
            List<NaturalHazard> results = new List<NaturalHazard>();
            while (query.HasMoreResults)
            {
                var response = await query.ReadNextAsync();
                
                results.AddRange(response.ToList());
            }

            return results;
        }

        public async Task UpdateItemAsync(string id, NaturalHazard item)
        {
            await _container.UpsertItemAsync<NaturalHazard>(item, new PartitionKey(id));
        }
    }
}