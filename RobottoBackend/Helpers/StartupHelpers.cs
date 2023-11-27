using Azure.Storage.Blobs;
using Microsoft.Azure.Cosmos;
using RobottoBackend.Data.Repositories;
using RobottoBackend.Services;

namespace RobottoBackend.Helpers
{
    public static class StartupHelpers
    {
        public static async Task<TestRepository> InitializeTestRepositoryAsync(IConfigurationSection configurationSection)
        {
            string databaseName = configurationSection.GetSection("DatabaseName").Value ?? "";
            string containerName = configurationSection.GetSection("TestContainerName").Value ?? "";
            var client = GetCosmosDbClient(configurationSection);
            
            TestRepository repository = new TestRepository(client, databaseName, containerName);
            DatabaseResponse databaseResponse = await client.CreateDatabaseIfNotExistsAsync(databaseName);
            await databaseResponse.Database.CreateContainerIfNotExistsAsync(containerName, "/id");

            return repository;
        }

        public static async Task<MissionRepository> InitializeMissionRepositoryAsync(IConfigurationSection configurationSection)
        {
            string databaseName = configurationSection.GetSection("DatabaseName").Value ?? "";
            string containerName = configurationSection.GetSection("MissionContainerName").Value ?? "";
            var client = GetCosmosDbClient(configurationSection);
            
            MissionRepository repository = new MissionRepository(client, databaseName, containerName);
            DatabaseResponse databaseResponse = await client.CreateDatabaseIfNotExistsAsync(databaseName);
            await databaseResponse.Database.CreateContainerIfNotExistsAsync(containerName, "/id");

            return repository;
        }

        public static async Task<ResourceRepository> InitializeResourceRepositoryAsync(IConfigurationSection configurationSection)
        {
            string databaseName = configurationSection.GetSection("DatabaseName").Value ?? "";
            string containerName = configurationSection.GetSection("ResourceContainerName").Value ?? "";
            var client = GetCosmosDbClient(configurationSection);
            
            ResourceRepository repository = new ResourceRepository(client, databaseName, containerName);
            DatabaseResponse databaseResponse = await client.CreateDatabaseIfNotExistsAsync(databaseName);
            await databaseResponse.Database.CreateContainerIfNotExistsAsync(containerName, "/id");

            return repository;
        }

        public static async Task<NaturalHazardRepository> InitializeNaturalHazardRepositoryAsync(IConfigurationSection configurationSection)
        {
            string databaseName = configurationSection.GetSection("DatabaseName").Value ?? "";
            string containerName = configurationSection.GetSection("NaturalHazardContainerName").Value ?? "";
            var client = GetCosmosDbClient(configurationSection);
            
            NaturalHazardRepository repository = new NaturalHazardRepository(client, databaseName, containerName);
            DatabaseResponse databaseResponse = await client.CreateDatabaseIfNotExistsAsync(databaseName);
            await databaseResponse.Database.CreateContainerIfNotExistsAsync(containerName, "/id");

            return repository;
        }

        public static async Task<DroneMetadataRepository> InitializeDroneMetadataRepositoryAsync(IConfigurationSection configurationSection)
        {
            string databaseName = configurationSection.GetSection("DatabaseName").Value ?? "";
            string containerName = configurationSection.GetSection("DroneMetadataContainerName").Value ?? "";
            var client = GetCosmosDbClient(configurationSection);
            
            DroneMetadataRepository repository = new DroneMetadataRepository(client, databaseName, containerName);
            DatabaseResponse databaseResponse = await client.CreateDatabaseIfNotExistsAsync(databaseName);
            await databaseResponse.Database.CreateContainerIfNotExistsAsync(containerName, "/id");

            return repository;
        }

        public static async Task<DroneTelemetryRepository> InitializeDroneTelemetryRepositoryAsync(IConfigurationSection configurationSection)
        {
            string databaseName = configurationSection.GetSection("DatabaseName").Value ?? "";
            string containerName = configurationSection.GetSection("DroneTelemetryContainerName").Value ?? "";
            var client = GetCosmosDbClient(configurationSection);
            
            DroneTelemetryRepository repository = new DroneTelemetryRepository(client, databaseName, containerName);
            DatabaseResponse databaseResponse = await client.CreateDatabaseIfNotExistsAsync(databaseName);
            await databaseResponse.Database.CreateContainerIfNotExistsAsync(containerName, "/id");

            return repository;
        }

        public static CosmosClient GetCosmosDbClient(IConfigurationSection configurationSection)
        {    
            string account = configurationSection.GetSection("Account").Value ?? "";
            string key = configurationSection.GetSection("Key").Value ?? "";

            CosmosClientOptions options = new ()
            {
                HttpClientFactory = () => new HttpClient(new HttpClientHandler()
                {
                    ServerCertificateCustomValidationCallback = HttpClientHandler.DangerousAcceptAnyServerCertificateValidator
                }),
                ConnectionMode = ConnectionMode.Gateway,
                LimitToEndpoint = true
            };
            
            return new CosmosClient(account, key, clientOptions: options);
        }

        public static async Task<AzuriteService> InitializeAzuriteClientInstanceAsync(IConfigurationSection configurationSection)
        {
            string connectionString = configurationSection.GetSection("ConnectionString").Value ?? "";;
            string containerName = configurationSection.GetSection("ContainerName").Value ?? "";

            var blobServiceClient = new BlobServiceClient(connectionString);

            if (!await ContainerExists(blobServiceClient, containerName))
            {
                await blobServiceClient.CreateBlobContainerAsync(containerName);
            }

            var blobContainerClient = new BlobContainerClient(connectionString, containerName);
            return new AzuriteService(blobContainerClient);
        }

        public static async Task<bool> ContainerExists(BlobServiceClient blobServiceClient, string containerName)
        {
            var containers = blobServiceClient.GetBlobContainersAsync(prefix: containerName);
            await foreach (var page in containers)
            {
                if (page.Name == containerName)
                    return true;
            }
            
            return false;
        }
    }
}