namespace RobottoBackend.Models
{
    public class NaturalHazard
    {
        public string Id { get; set; } = Guid.NewGuid().ToString();
        public string MissionId { get; set; } = "";
        public NaturalHazardType Type { get; set; }
        public IEnumerable<Resource> Resources { get; set; } = new List<Resource>();
        public IEnumerable<string> SelectionArea { get; set; } = new List<string>(); // TODO: fix type
    }

    public enum NaturalHazardType
    {
        Default
    }
}