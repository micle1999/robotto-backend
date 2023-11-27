namespace RobottoBackend.Models
{
    public class DroneMetadata
    {
        public Guid Id { get; set; }
        public Guid DroneId { get; set; }
        public Guid ActiveMissionId { get; set; }
    }
}