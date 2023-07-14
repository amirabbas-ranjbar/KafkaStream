using Confluent.Kafka;
using Confluent.Kafka.Admin;
using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;
using Streamiz.Kafka.Net;
using Streamiz.Kafka.Net.SerDes;
using Streamiz.Kafka.Net.Table;

namespace WebApplication1.Controllers;

[ApiController]
[Route("api/[controller]")]
public class KafkaController : ControllerBase
{
    private readonly IConfiguration _configuration;
    private readonly ProducerConfig _producerConfig;
    private readonly string _bootstrapServers;
    private readonly string _orderTopic;
    private readonly string _customerTopic;
    private readonly string _productTopic;
    public KafkaController(IConfiguration configuration)
    {
        _configuration = configuration;
        var kafkaOptions = configuration.GetSection("KafkaOptions").Get<Dictionary<string, string>>();
        _producerConfig = new ProducerConfig
        {
            BootstrapServers = kafkaOptions["BoostrapServer"].ToString(),
            Acks = Acks.All,
            EnableBackgroundPoll = true,
            LingerMs = 3000,
            EnableIdempotence = false,
            CompressionType = CompressionType.Lz4,
            MaxInFlight = 1_000_000,
            RequestTimeoutMs = 20,
            RetryBackoffMs = 1000,
            BatchSize = 6_000_000,
            BatchNumMessages = 3000,
        };
        _bootstrapServers = kafkaOptions["BoostrapServer"].ToString();
        _orderTopic = kafkaOptions["OrderTopic"].ToString();
        _customerTopic = kafkaOptions["CustomerTopic"].ToString();
        _productTopic = kafkaOptions["ProductTopic"].ToString();
    }

    [HttpPost("CreateStream/{orderId}")]
    public async Task<IActionResult> CreateTopics(int orderId)
    {
        var product = Product.Create();
        var customer = Customer.Create();
        var order = new Order(orderId, product.id, customer.id);
        await ProduceMessageAsync(_productTopic, product.id, product);
        await ProduceMessageAsync(_customerTopic, customer.id, customer);
        await ProduceMessageAsync(_orderTopic, order.order_id.ToString(), order);
        KafkaExtensions.StartStreamProcessing(_configuration);

        return Ok(new
        {
            Customer = customer,
            Product = product,
            Order = order
        });
    }
    private async Task ProduceMessageAsync(string topic, string key, object value)
    {
        var config = new ProducerConfig
        {
            BootstrapServers = _bootstrapServers
        };
        using var producer = new ProducerBuilder<string, string>(config)
            .SetValueSerializer(Serializers.Utf8)
            .SetKeySerializer(Serializers.Utf8)
            .Build();

        string serializedValue = JsonConvert.SerializeObject(value);

        var deliveryReport = await producer.ProduceAsync(topic, new Message<string, string>
        {
            Key = key,
            Value = serializedValue
        });

        Console.WriteLine($"Message delivered to '{deliveryReport.TopicPartitionOffset}'.");
    }
}