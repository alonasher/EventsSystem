using EventsConsumer;
using EventsConsumer.Configuration;

var builder = Host.CreateApplicationBuilder(args);

builder.Services.Configure<KafkaSettings>(builder.Configuration.GetSection("Kafka"));
builder.Services.Configure<InfluxDbSettings>(builder.Configuration.GetSection("InfluxDB"));
builder.Services.AddHostedService<Worker>();

var host = builder.Build();
await host.RunAsync();
