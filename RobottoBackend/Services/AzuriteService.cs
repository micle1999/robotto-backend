using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Azure.Storage.Blobs;

namespace RobottoBackend.Services
{
    
    public class AzuriteService
    {
        private readonly IConfiguration _configuration;
        private readonly BlobContainerClient _client;
        private readonly ILogger<AzuriteService> _logger;
        private readonly string _containerName = "testcontainer";
        private string _connectionString;

        public AzuriteService(IConfiguration configuration, ILogger<AzuriteService> logger)
        {
            _logger = logger;
            _configuration = configuration;
            _connectionString = configuration["Azurite:AzuriteConnection"] ?? "";
            _client = new BlobContainerClient(_connectionString, _containerName);
        }

        public async Task EnsureContainerCreated()
        {
            var blobServiceClient = new BlobServiceClient(_connectionString);

            if (!await ContainerExists(blobServiceClient))
            {
                await blobServiceClient.CreateBlobContainerAsync(_containerName);
            }
        }        

        public async Task<bool> CreateBlobFileAsync(string filename, byte[] data)
        {
            // Check if a blob with a given filename exists - if so throw an exception
            if(GetBlob(filename).Exists())
            {
                throw new Exception("Blob already exists");
            }
            
            try
            {
                // Convert the byte data into a stream so we can upload it into azurite
                await using var memoryStream = new MemoryStream(data, false);
                var response = await _client.UploadBlobAsync(filename, memoryStream);
            }
            catch(Exception exception)
            {
                // do something
                return false;
            }
        
            return true;
        }

        public async Task<Stream?> GetBlobFileAsync(string filename)
        {
            // Gets a given blob and returns null if it doesn't exist
            var blob = GetBlob(filename);
            if(!blob.Exists())
            {
                return null;
            }
            // Downloads the blob content and returns a stream
            var blobContent = await blob.DownloadContentAsync();
            return blobContent.Value.Content.ToStream();
        }

        public async Task<bool> DeleteBlobFileAsync(string filename)
        {
            // Deletes a blob if it exists
            return await _client.DeleteBlobIfExistsAsync(filename);
        }

        public int GetBlobCount()
        {
            // Gets the number of blobs in a given container
            _logger.LogInformation("Logging Get Blob Count");

            return _client.GetBlobs().Count();
        }

        private BlobClient GetBlob(string filename)
        {
            // Check and returns a given blob by its filename
            return _client.GetBlobClient(filename);
        }

        private async Task<bool> ContainerExists(BlobServiceClient blobServiceClient)
        {
            var containers = blobServiceClient.GetBlobContainersAsync(prefix: _containerName);
            await foreach (var page in containers)
            {
                if (page.Name == _containerName)
                    return true;
            }
            
            return false;
        }
    }
}
