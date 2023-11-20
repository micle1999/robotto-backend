namespace RobottoBackend.Services
{
    public interface IAzuriteService
    {
        Task<bool> CreateBlobFileAsync(string filename, byte[] data);
        Task<bool> CreateBlobFileAsync(string filename, Stream stream);
        Task<Stream?> GetBlobFileAsync(string filename);
        Task<bool> DeleteBlobFileAsync(string filename);
        int GetBlobCount();
        IEnumerable<string> GetBlobNames();
    } 
}