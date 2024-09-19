using CDC_PoC.CDC.Services;
using CDC_PoC.Config;
using CDC_PoC.Search;
using Elastic.Clients.Elasticsearch;
using Microsoft.Extensions.Options;
using Refit;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddControllers();

// Add Swagger services
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

builder.Services.Configure<AppConfig>(builder.Configuration);

builder.Services.AddLogging(x => x.AddConsole());

builder.Services.AddScoped<ElasticsearchClient>(serviceProvider =>
{
    var appConfig = serviceProvider.GetRequiredService<IOptions<AppConfig>>().Value;
    var settings = new ElasticsearchClientSettings(new Uri(appConfig.ElasticsearchConfiguration.ElasticClientHost));
    return new ElasticsearchClient(settings);
});

builder.Services.AddScoped<ICustomerService, CustomerService>();
builder.Services.AddScoped<IElasticCudService, ElasticCudService>();
builder.Services.AddScoped<ISearchService, SearchService>();
builder.Services.AddHostedService<KafkaConsumer>();

var customerSettingsUrl = builder.Configuration["CustomerSettingConfiguration:ApiUrl"];
if (string.IsNullOrWhiteSpace(customerSettingsUrl))
{
    throw new InvalidOperationException("The base URI for the CustomerSettingService cannot be null or empty.");   
}
var refitHttpClient = new HttpClient() { BaseAddress = new Uri(customerSettingsUrl) };
builder.Services.AddScoped<ICustomerSettingsClient>(_ => RestService.For<ICustomerSettingsClient>(refitHttpClient));


var app = builder.Build();

if (app.Environment.IsDevelopment())
{
    app.UseDeveloperExceptionPage();
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.MapControllers();

app.Run();