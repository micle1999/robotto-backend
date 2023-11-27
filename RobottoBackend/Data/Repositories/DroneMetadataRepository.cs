using AutoFixture;
using Microsoft.Azure.Cosmos;
using RobottoBackend.Models;

namespace RobottoBackend.Data.Repositories
{
    public class DroneMetadataRepository : IDroneMetadataRepository
    {
        private readonly Container _container;

        public DroneMetadataRepository(
            CosmosClient dbClient,
            string databaseName,
            string containerName)
        {
            _container = dbClient.GetContainer(databaseName, containerName);
        }
        
        public async Task AddItemAsync(DroneMetadata item)
        {
            await _container.CreateItemAsync<DroneMetadata>(item, new PartitionKey(item.Id));
        }

        public async Task DeleteItemAsync(string id)
        {
            await _container.DeleteItemAsync<DroneMetadata>(id, new PartitionKey(id));
        }

        public async Task<DroneMetadata> GetItemAsync(string id)
        {
            try
            {
                ItemResponse<DroneMetadata> response = await _container.ReadItemAsync<DroneMetadata>(id, new PartitionKey(id));
                return response.Resource;
            }
            catch(CosmosException ex) when (ex.StatusCode == System.Net.HttpStatusCode.NotFound)
            { 
                return null;
            }

        }

        public async Task<IEnumerable<DroneMetadata>> GetItemsAsync(string queryString)
        {
            var query = _container.GetItemQueryIterator<DroneMetadata>(new QueryDefinition(queryString));
            List<DroneMetadata> results = new List<DroneMetadata>();
            while (query.HasMoreResults)
            {
                var response = await query.ReadNextAsync();
                
                results.AddRange(response.ToList());
            }

            return results;
        }

        public async Task UpdateItemAsync(string id, DroneMetadata item)
        {
            await _container.UpsertItemAsync<DroneMetadata>(item, new PartitionKey(id));
        }

        public async Task SeedMockData(int limit)
        {
            var fixture = new Fixture();

            for (int i = 0; i < limit; i++)
            {
                var item = fixture.Create<DroneMetadata>();
                await AddItemAsync(item);
            }
        }
    }
}