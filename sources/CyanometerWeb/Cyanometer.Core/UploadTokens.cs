namespace Cyanometer.Core
{
    /// <summary>
    /// Contains tokens required for upload
    /// </summary>
    public record UploadTokens
    {
        public static UploadTokens Instance { get; private set; }
        public string Ljubljana { get; }
        public string Wroclaw { get; }
        public string Millstatt { get; }
        public string Geneva { get; }
        public static void Init(string ljubljana, string wroclaw, string millstat, string geneva)
        {
            Instance = new UploadTokens(ljubljana, wroclaw, millstat, geneva);
        }
        private UploadTokens(string ljubljana, string wroclaw, string millstatt, string geneva)
        {
            Ljubljana = ljubljana;
            Wroclaw = wroclaw;
            Millstatt = millstatt;
            Geneva = geneva;
        }
    }
}
