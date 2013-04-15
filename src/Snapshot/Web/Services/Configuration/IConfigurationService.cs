namespace Web.Services.Configuration
{
    public interface IConfigurationService
    {
        string this[string key] { get; }
        ConfigurationKeys Keys { get; }
    }
}