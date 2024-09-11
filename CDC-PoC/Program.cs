using CDC_PoC.Config;
using CDC_PoC.Services;
using Microsoft.Extensions.Options;
using Nest;
using Refit;

var builder = WebApplication.CreateBuilder(args);

builder.Services.Configure<AppConfig>(builder.Configuration);

builder.Services.AddLogging(x => x.AddConsole());

builder.Services.AddScoped<IElasticClient>(serviceProvider =>
{
    var appConfig = serviceProvider.GetRequiredService<IOptions<AppConfig>>().Value;
    return new ElasticClient(new ConnectionSettings(new Uri(appConfig.ElasticsearchConfiguration.ElasticClientHost)));
});
builder.Services.AddScoped<IDocumentTenantIdInjector, DocumentTenantIdInjector>();
builder.Services.AddScoped<IElasticCudService, ElasticCudService>();
builder.Services.AddHostedService<KafkaConsumer>();

var customerSettingsUrl = builder.Configuration["CustomerSettingConfiguration:ApiUrl"];
if (string.IsNullOrWhiteSpace(customerSettingsUrl))
{
    throw new InvalidOperationException("The base URI for the CustomerSettingService cannot be null or empty.");   
}
var refitHttpClient = new HttpClient() { BaseAddress = new Uri(customerSettingsUrl) };
builder.Services.AddScoped<ICustomerSettingRestServices>(_ => RestService.For<ICustomerSettingRestServices>(refitHttpClient));


var app = builder.Build();

app.UseHttpsRedirection();

app.MapGet("/home", () => "Welcome to CDC PoC")
    .WithName("Home")
    .WithOpenApi();

app.Run();