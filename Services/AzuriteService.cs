using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Azure.Storage.Blobs;

namespace RobottoBackend.Services
{
    
    public class AzuriteService
    {
        private readonly IConfiguration configuration;
        private readonly BlobContainerClient client;

        private readonly ILogger<AzuriteService> logger;

        public AzuriteService(IConfiguration _configuration, ILogger<AzuriteService> _logger)
        {
            this.logger = _logger;
            this.configuration = _configuration;
            this.client = new BlobContainerClient(configuration["Azurite:AzuriteConnection"], "testContainer");
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
                var response = await client.UploadBlobAsync(filename, memoryStream);
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
            return await client.DeleteBlobIfExistsAsync(filename);
        }

        public int GetBlobCount()
        {
            // Gets the number of blobs in a given container
            logger.LogInformation("Logging Get Blob Count");
            return client.GetBlobs().Count();
        }

        private BlobClient GetBlob(string filename)
        {
            // Check and returns a given blob by its filename
            return client.GetBlobClient(filename);
        }
    }
}
