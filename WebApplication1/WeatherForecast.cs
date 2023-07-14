using Confluent.Kafka;
using Shared;

namespace WebApplication1;

public class KafkaConsumerService : BackgroundService
{
    protected override async Task ExecuteAsync(CancellationToken stoppingToken)
    {
        while (!stoppingToken.IsCancellationRequested)
        {
            await KafkaExtensions.StartStreamProcessing();
        }
    }
}