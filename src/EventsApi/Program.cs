using EventsApi;
using EventsApi.Configurations;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddControllers();

builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();
builder.Services.AddAuthorization();

// Register services via configuration extensions
builder.Services.AddKafkaProducer(builder.Configuration);
builder.Services.AddInfluxDb(builder.Configuration);
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