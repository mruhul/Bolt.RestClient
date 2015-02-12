namespace Bolt.RestClient.Impl
{
    internal class RestClientSettings : IRestClientSettings
    {
        public RestClientSettings()
        {
            DefaultConnectionLimit = 1000;
            AutomaticDecompression = true;
        }

        public int DefaultConnectionLimit { get; internal set; }
        public bool AutomaticDecompression { get; internal set; }
        public string Proxy { get; internal set; }
        public bool BypassOnLocal { get; internal set; }
    }
}