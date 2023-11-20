using Azure.Storage.Blobs;

namespace RobottoBackend.Services
{
    
    public class AzuriteService : IAzuriteService
    {
        private readonly BlobContainerClient _client;

        public AzuriteService(BlobContainerClient client)
        {            
            _client = client;
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
            catch
            {
                // do something
                return false;
            }
        
            return true;
        }

        public async Task<bool> CreateBlobFileAsync(string filename, Stream stream)
        {
            // Check if a blob with a given filename exists - if so throw an exception
            if(GetBlob(filename).Exists())
            {
                throw new Exception("Blob already exists");
            }
            
            try
            {
                var response = await _client.UploadBlobAsync(filename, stream);
            }
            catch
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
            // TODO: make async
            // Gets the number of blobs in a given container
            return _client.GetBlobs().Count();
        }

        public IEnumerable<string> GetBlobNames()
        {
            // TODO: make async
            return _client.GetBlobs().Select(b => b.Name);
        }

        private BlobClient GetBlob(string filename)
        {
            // Check and returns a given blob by its filename
            return _client.GetBlobClient(filename);
        }
    }
}
