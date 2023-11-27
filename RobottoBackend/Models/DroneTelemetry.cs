namespace RobottoBackend.Models
{
    public class DroneTelemetry
    {
        public string Id { get; set; } = Guid.NewGuid().ToString();
        public string MissionId { get; set; } = "";
        public string HomePointAltitude { get; set; }  = ""; // TODO: fix the type
        public string Gimbal { get; set; }  = ""; // TODO: fix the type
        public string GpsFused { get; set; }  = ""; // TODO: fix the type
        public string Quaternion { get; set; }  = ""; // TODO: fix the type
    }
}