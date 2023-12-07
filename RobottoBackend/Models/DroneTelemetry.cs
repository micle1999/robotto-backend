using Newtonsoft.Json;

namespace RobottoBackend.Models
{
    public class DroneTelemetry
    {
        [JsonProperty(PropertyName = "id")]
        public string Id { get; set; } = Guid.NewGuid().ToString();
        public string MissionId { get; set; } = "";
        public float HomePointAltitude { get; set; }
        public Gimbal Gimbal { get; set; }
        public GpsFused GpsFused { get; set; }
        public Quaternion Quaternion { get; set; }

        public DroneTelemetry(string id, string missionId, float homePointAltitude, Gimbal gimbal, GpsFused gpsFused, 
            Quaternion quaternion)
        {
            Id = id;
            MissionId = missionId;
            HomePointAltitude = homePointAltitude;
            Gimbal = gimbal;
            GpsFused = gpsFused;
            Quaternion = quaternion;
        }
    }

    public class Gimbal
    {
        public int Mode { get; set; }
        public float Pitch { get; set; }
        public float Roll { get; set; }
        public int Status { get; set; }
        public float Yaw { get; set; }

        public Gimbal(int mode, float pitch, float roll, int status, float yaw)
        {
            Mode = mode;
            Pitch = pitch;
            Roll = roll;
            Status = status;
            Yaw = yaw;
        }
    }

    public class GpsFused
    {
        public float Altitude { get; set; }
        public Coordinates Coordinates { get; set; }
        public int VisibleSatelliteNumber { get; set; }

        public GpsFused(float altitude, Coordinates coordinates, int visibleSatelliteNumber)
        {
            Altitude = altitude;
            Coordinates = coordinates;
            VisibleSatelliteNumber = visibleSatelliteNumber;
        }
    }

    public class Quaternion
    {
        public float Q1 { get; set; }
        public float Q2 { get; set; }
        public float Q3 { get; set; }
        public float Q4 { get; set; }

        public Quaternion(float q1, float q2, float q3, float q4)
        {
            Q1 = q1;
            Q2 = q2;
            Q3 = q3;
            Q4 = q4;
        }
    }
}