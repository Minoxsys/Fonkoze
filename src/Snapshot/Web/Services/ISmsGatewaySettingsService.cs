namespace Web.Services
{
    public interface ISmsGatewaySettingsService
    {
        string SmsGatewayUrl { get; }
        string SmsGatewayUserName { get; }
        string SmsGatewayPassword { get; }
        string SmsGatewayFrom { get; }
        string SmsGatewayTestMode { get; }
        string SmsGatewayDebugMode { get; }
    }
}