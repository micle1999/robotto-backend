namespace RobottoBackend.Models
{
    public class Mission
    {
        public string Id { get; set; } = Guid.NewGuid().ToString();
        public string Name { get; set; } = "";
        public MissionType Type { get; set; }
        public DateTime Created { get; set; }
        public Coordinates Coordinates { get; set; }
        public bool IsActive { get; set; }

        public Mission(string id, string name, MissionType type, DateTime created, Coordinates coordinates, bool isActive)
        {
            Id = id;
            Name = name;
            Type = type;
            Created = created;
            Coordinates = coordinates;
            IsActive = isActive;
        }
    }

    public enum MissionType
    {
        Unknown,
        Fire
    }
}