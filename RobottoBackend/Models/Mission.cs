namespace RobottoBackend.Models
{
    public class Mission
    {
        public Guid Id { get; set; }
        public string Name { get; set; } = "";
        public MissionType Type { get; set; }
        public DateTime Created { get; set; }
        public string Coordinates { get; set; } = ""; // TODO: fix the type
    }

    public enum MissionType
    {
        Default
    }
}