namespace Cyanometer.AirQuality.Services;

public record SabraAirQualityCredentials
{
    public static Credentials Instance { get; private set; }
    public static void Init(string username, string password)
    {
        Instance = new Credentials(username, password);
    }
}