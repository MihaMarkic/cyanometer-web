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
        public string Dresden { get; }
        public string Geneva { get; }
        public static void Init(string ljubljana, string wroclaw, string dresden, string geneva)
        {
            Instance = new UploadTokens(ljubljana, wroclaw, dresden, geneva);
        }
        private UploadTokens(string ljubljana, string wroclaw, string dresden, string geneva)
        {
            Ljubljana = ljubljana;
            Wroclaw = wroclaw;
            Dresden = dresden;
            Geneva = geneva;
        }
    }
}
