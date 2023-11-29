namespace RobottoBackend.Models
{
    public class NaturalHazard
    {
        public string Id { get; set; } = Guid.NewGuid().ToString();
        public string MissionId { get; set; } = "";
        public NaturalHazardType Type { get; set; }
        public IEnumerable<Resource> Resources { get; set; } = new List<Resource>();
        public IEnumerable<Coordinates> SelectionArea { get; set; } = new List<Coordinates>();

        public NaturalHazard(string id, string missionId, NaturalHazardType type, IEnumerable<Resource> resources,
            IEnumerable<Coordinates> selectionArea)
        {
            Id = id;
            MissionId = missionId;
            Type = type;
            Resources = resources;
            SelectionArea = selectionArea;
        }
    }

    public enum NaturalHazardType
    {
        Unknown,
        Fire,
        Flood,
        Earthquake
    }
}