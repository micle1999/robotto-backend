namespace RobottoBackend.Models
{
    public class Resource
    {
        public string Id { get; set; } = Guid.NewGuid().ToString();
        public string MissionId { get; set; } = "";
        public ResourceType Type { get; set; }
        public DateTime Created { get; set; }
        public bool Raw { get; set; }
        public string ResourceUri { get; set; } = "";
    }

    public enum ResourceType
    {
        Default
    }
}