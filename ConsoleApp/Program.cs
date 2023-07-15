// See https://aka.ms/new-console-template for more information

using Shared;

Console.WriteLine(KafkaOptionsValues.DeviceInfo);
await KafkaExtensions.StartStreamProcessing();