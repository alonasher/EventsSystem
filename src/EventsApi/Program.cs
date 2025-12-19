using EventsApi;
using InfluxDB.Client; 

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddControllers();

builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

builder.Services.AddSingleton<IKafkaProducerService, KafkaProducerService>();


builder.Services.AddSingleton<IInfluxDBClient>(sp =>
{
    var config = sp.GetRequiredService<IConfiguration>();
    return InfluxDBClientFactory.Create(
        config["InfluxDB:Url"], 
        config["InfluxDB:Token"]);
});
// ----------------------------------------

var app = builder.Build();

if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseAuthorization();

app.MapControllers();

app.Run();