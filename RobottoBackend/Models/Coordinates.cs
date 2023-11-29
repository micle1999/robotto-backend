namespace RobottoBackend.Models
{
    public class Coordinates
    {
        public float Longitude { get; set; }
        public float Latitude { get; set; }

        public Coordinates(float longitude, float latitude)
        {
            Longitude = longitude;
            Latitude = latitude;
        }
    }
}
