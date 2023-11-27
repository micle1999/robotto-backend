namespace RobottoBackend.Models
{
    public class Resource
    {
        public Guid Id { get; set; }
        public Guid MissionId { get; set; }
        public Guid ResourceId { get; set; }
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