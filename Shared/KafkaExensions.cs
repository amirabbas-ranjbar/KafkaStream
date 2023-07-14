using Streamiz.Kafka.Net;
using Streamiz.Kafka.Net.SerDes;
using Streamiz.Kafka.Net.Stream;
using Streamiz.Kafka.Net.Table;

namespace Shared;

public static class KafkaExtensions
{
    public static async Task StartStreamProcessing()
    {
        CancellationTokenSource source = new();
        var kafkaOptions = KafkaOptionsValues.KafkaOptions;
        var config = new StreamConfig();
        config.ApplicationId = kafkaOptions["ApplicationId"].ToString();
        config.ClientId = kafkaOptions["ClientId"].ToString();
        config.BootstrapServers = kafkaOptions["BoostrapServer"].ToString();
        config.AutoOffsetReset = Confluent.Kafka.AutoOffsetReset.Earliest;

        var topology = GetTopology(kafkaOptions["OrderTopic"].ToString(), kafkaOptions["CustomerTopic"].ToString(),
            kafkaOptions["CustomerStoreTopic"].ToString(), kafkaOptions["ProductTopic"].ToString(),
            kafkaOptions["ProductStoreTopic"].ToString(), kafkaOptions["OrderEnrichedTopic"].ToString());

        KafkaStream stream = new(topology, config);
        await stream.StartAsync(source.Token);
    }
    static Topology GetTopology(string orderTopic, string customerTopic, string customerStoreTopic, string productTopic,
        string productStoreTopic, string orderEnrichedTopic)
    {
        StreamBuilder builder = new();
        var orderStream = builder.Stream<string, Order, StringSerDes, JsonSerDes<Order>>(orderTopic);
        var customers = builder.GlobalTable<string, Customer, StringSerDes, JsonSerDes<Customer>>(
            customerTopic, InMemory.As<string, Customer>(customerStoreTopic));

        var products = builder.GlobalTable<string, Product, StringSerDes, JsonSerDes<Product>>(
            productTopic, InMemory.As<string, Product>(productStoreTopic));

        var customerOrderStream = orderStream.Join(customers,
            (orderId, order) => order.customer_id,
            (order, customer) => new CustomerOrder(customer, order));

        var enrichedOrderStream = customerOrderStream.Join(products,
            (orderId, customerOrder) => customerOrder.Order.product_id,
            (customerOrder, product) => EnrichedOrderBuilder.Build(customerOrder, product));

        enrichedOrderStream.To<StringSerDes, JsonSerDes<EnrichedOrder>>(orderEnrichedTopic);
        return builder.Build();
    }
}