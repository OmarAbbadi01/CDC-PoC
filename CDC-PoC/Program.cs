using CDC_PoC.Services;
using Nest;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddLogging(x => x.AddConsole());

builder.Services.AddSingleton<IElasticClient>(_ => new ElasticClient(
    new ConnectionSettings(new Uri("http://localhost:9200")))
);
builder.Services.AddSingleton<IElasticCudService, ElasticCudService>();
builder.Services.AddHostedService<KafkaConsumer>();

var app = builder.Build();

app.UseHttpsRedirection();

app.MapGet("/home", () => "Welcome to CDC PoC")
    .WithName("Home")
    .WithOpenApi();

app.Run();