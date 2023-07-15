namespace Shared;

public class KafkaOptionsValues
{
    // public static string DeviceInfo = Guid.NewGuid().ToString().GetHashCode().ToString("x");
    public static string DeviceInfo = "AmirAbbas";

    public static readonly Dictionary<string, string> KafkaOptions = new Dictionary<string, string>()
    {
        {
            "BoostrapServer", "localhost:9094"
        },
        {
            "ApplicationId", DeviceInfo
        },
        {
            "ClientId", $"{DeviceInfo}-client"
        },
        {
            "OrderTopic", "Orders"
        },
        {
            "CustomerTopic", "Customers"
        },
        {
            "CustomerStoreTopic", "customer-store"
        },
        {
            "ProductStoreTopic", "product-store"
        },
        {
            "ProductTopic", "Products"
        },
        {
            "OrderEnrichedTopic", "OrderEnriched"
        },
        {
            "GroupId", "test"
        },
    };
}