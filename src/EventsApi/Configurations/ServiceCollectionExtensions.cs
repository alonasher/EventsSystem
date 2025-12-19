using InfluxDB.Client;
using Microsoft.Extensions.Options;

namespace EventsApi.Configurations
{
    public static class ServiceCollectionExtensions
    {
        public static IServiceCollection AddKafkaProducer(this IServiceCollection services, IConfiguration configuration)
        {
            services.Configure<KafkaSettings>(configuration.GetSection("Kafka"));
            services.AddSingleton<IKafkaProducerService, KafkaProducerService>();
            return services;
        }

        public static IServiceCollection AddInfluxDb(this IServiceCollection services, IConfiguration configuration)
        {
            services.Configure<InfluxDbSettings>(configuration.GetSection("InfluxDB"));

            services.AddSingleton<IInfluxDBClient>(sp =>
            {
                var influx = sp.GetRequiredService<IOptions<InfluxDbSettings>>().Value;
                var options = new InfluxDBClientOptions(influx.Url)
                {
                    Token = influx.Token
                };
                return new InfluxDBClient(options);
            });

            services.AddSingleton<IInfluxQueryService, InfluxQueryService>();

            return services;
        }
    }
}
