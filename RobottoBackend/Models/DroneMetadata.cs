namespace RobottoBackend.Models
{
    public class DroneMetadata
    {
        public string Id { get; set; } = Guid.NewGuid().ToString();
        public string DroneId { get; set; } = "";

        public DroneMetadata(string id, string droneId)
        {
            Id = id;
            DroneId = droneId;
        }
    }
}