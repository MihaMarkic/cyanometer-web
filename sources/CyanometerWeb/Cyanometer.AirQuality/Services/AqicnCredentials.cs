namespace Cyanometer.AirQuality.Services;

public record AqicnCredentials
{
    public static AqicnCredentials Instance { get; private set; }
    public string Token { get; }
    public static void Init(string token)
    {
        Instance = new AqicnCredentials(token);
    }
    private AqicnCredentials(string token)
    {
        Token = token;
    }
}
