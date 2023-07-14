using Newtonsoft.Json;
using Streamiz.Kafka.Net;
using Streamiz.Kafka.Net.SerDes;
using Streamiz.Kafka.Net.Stream;
using Streamiz.Kafka.Net.Table;
using WebApplication1.Controllers;

namespace WebApplication1;

public static class KafkaExtensions
{
    public static async void StartStreamProcessing(IConfiguration configuration)
    {
        CancellationTokenSource source = new();
        var kafkaOptions = configuration.GetSection("KafkaOptions").Get<Dictionary<string, string>>();
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

public class MyBackgroundService : BackgroundService
{
    private readonly IConfiguration _configuration;
    public MyBackgroundService(IConfiguration configuration)
    {
        _configuration = configuration;
    }
    protected override Task ExecuteAsync(CancellationToken stoppingToken)
    {
        while (!stoppingToken.IsCancellationRequested)
        {
            // Your background task logic here
            KafkaExtensions.StartStreamProcessing(_configuration);
        }

        return Task.CompletedTask;
    }
    public void Configure(IApplicationBuilder app, IWebHostEnvironment env)
    {
        using (var serviceScope = app.ApplicationServices.CreateScope())
        {
            var backgroundService = serviceScope.ServiceProvider.GetRequiredService<MyBackgroundService>();
            backgroundService.StartAsync(CancellationToken.None).GetAwaiter().GetResult();
        }
    }
}