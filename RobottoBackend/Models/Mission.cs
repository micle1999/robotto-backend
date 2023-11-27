namespace RobottoBackend.Models
{
    public class Mission
    {
        public string Id { get; set; } = Guid.NewGuid().ToString();
        public string Name { get; set; } = "";
        public MissionType Type { get; set; }
        public DateTime Created { get; set; }
        public string Coordinates { get; set; } = ""; // TODO: fix the type
        public bool IsActive { get; set; }
    }

    public enum MissionType
    {
        Default
    }
}