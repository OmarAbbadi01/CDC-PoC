using CDC_PoC.Config;
using CDC_PoC.CustomerSettings;
using CDC_PoC.Elastic;
using CDC_PoC.Search;
using Elastic.Clients.Elasticsearch;
using Microsoft.Extensions.Options;
using R365.Context;
using R365.Context.Extensions;
using R365.Libraries.Security.Azure;
using R365.Messaging;
using R365.Messaging.Extensions;
using R365.Messaging.Subscriber;
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

builder.Services.AddScoped<ICustomerSettingsService, CustomerSettingsService>();
builder.Services.AddScoped<IElasticCudService, ElasticCudService>();
builder.Services.AddScoped<ISearchService, SearchService>();
// builder.Services.AddHostedService<KafkaConsumer>();

var customerSettingsUrl = builder.Configuration["CustomerSettingConfiguration:ApiUrl"];
if (string.IsNullOrWhiteSpace(customerSettingsUrl))
{
    throw new InvalidOperationException("The base URI for the CustomerSettingService cannot be null or empty.");   
}
var refitHttpClient = new HttpClient() { BaseAddress = new Uri(customerSettingsUrl) };
builder.Services.AddScoped<ICustomerSettingsClient>(_ => RestService.For<ICustomerSettingsClient>(refitHttpClient));

builder.Configuration.AddAzureResourceSecurity();
builder.Services.AddAzureResourceSecurity();
builder.Services.AddMessaging(options =>
{
    var configSection = builder.Configuration.GetSection("ServiceBusConfig");
    options.ConnectionString = configSection.GetValue<string>("ConnectionString");

    // This is the flag that determines whether to use kafka or event hub.
    // For local development, set this to true to use kafka.
    options.UseLocalEventStreams = configSection.GetValue<bool>("UseLocalEventStreams");
    
    options.EventStreamOptions = configSection.GetSection(nameof(MessagingOptions.EventStreamOptions)).Get<EventStreamOptions>();
    options.EventStreamOptions.Subscriptions = configSection.GetSection(nameof(MessagingOptions.EventStreamOptions)).GetSection(nameof(MessagingOptions.EventStreamOptions.Subscriptions)).GetEventSubscriptionOptionsFromConfig();
});

// EnvironmentProvider
builder.Services.Configure<ContextOptions>(o => o.EnvironmentName = string.Empty);
builder.Services.AddR365EnvironmentProvider();

// Add R365.Context
builder.Services.AddR365Context();

// Add logging
builder.Services.AddLogging(configure => configure.AddConsole());


var app = builder.Build();

if (app.Environment.IsDevelopment())
{
    app.UseDeveloperExceptionPage();
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.MapControllers();

app.Run();