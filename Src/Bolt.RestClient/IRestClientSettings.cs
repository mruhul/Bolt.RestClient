namespace Bolt.RestClient
{
    public interface IRestClientSettings
    {
        int DefaultConnectionLimit { get; }
        bool AutomaticDecompression { get; }
        string Proxy { get; }
        bool BypassOnLocal { get; }
    }
}