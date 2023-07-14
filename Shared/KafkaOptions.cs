namespace Shared;

public class KafkaOptionsValues
{
    public static readonly Dictionary<string, string> KafkaOptions = new Dictionary<string, string>()
    {
        {
            "BoostrapServer", "localhost:9094"
        },
        {
            "ApplicationId", "amirabbas"
        },
        {
            "ClientId", "amirabbas-client"
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
    };
}