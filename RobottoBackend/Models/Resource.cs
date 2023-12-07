using Newtonsoft.Json;

namespace RobottoBackend.Models
{
    public class Resource
    {
        [JsonProperty(PropertyName = "id")]
        public string Id { get; set; } = Guid.NewGuid().ToString();
        public string MissionId { get; set; } = "";
        public ResourceType Type { get; set; }
        public DateTime Created { get; set; }
        public bool IsRaw { get; set; }
        public string ResourceUri { get; set; } = "";

        public Resource(string id, string missionId, ResourceType type, DateTime created, bool isRaw, string resourceUri)
        {
            Id = id;
            MissionId = missionId;
            Type = type;
            Created = created;
            IsRaw = isRaw;
            ResourceUri = resourceUri;
        }
    }

    public enum ResourceType
    {
        Unknown,
        Image,
        Video
    }
}