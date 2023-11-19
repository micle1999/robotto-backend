namespace RobottoBackend.Services
{
    public interface IAzuriteService
    {
        public Task<bool> CreateBlobFileAsync(string filename, byte[] data);
        public Task<Stream?> GetBlobFileAsync(string filename);
        public Task<bool> DeleteBlobFileAsync(string filename);
        public int GetBlobCount();
    } 
}