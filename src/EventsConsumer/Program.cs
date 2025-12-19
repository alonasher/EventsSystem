using EventsConsumer;
using EventsConsumer.Configuration;

var builder = Host.CreateApplicationBuilder(args);

builder.Services
    .Configure<KafkaSettings>(builder.Configuration.GetSection("Kafka"))
    .Configure<InfluxDbSettings>(builder.Configuration.GetSection("InfluxDB"))
    .AddHostedService<Worker>();

var host = builder.Build();
await host.RunAsync();
