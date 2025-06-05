using InCommoditiesTechChallenge;
using InCommoditiesTechChallenge.Models;
using InCommoditiesTechChallenge.NotificationSenders;
using InCommoditiesTechChallenge.Scrapers;
using MailKit.Net.Smtp;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;

#region Configurations
var configuration = new ConfigurationBuilder()
    .SetBasePath(AppContext.BaseDirectory)
    .AddJsonFile("appsettings.json", optional: true, reloadOnChange: true)
    .Build();

//Logging setup
using var loggerFactory = LoggerFactory.Create(builder =>
{
    builder
        .AddFilter("Microsoft", LogLevel.Warning)
        .AddFilter("System", LogLevel.Warning)
        .AddFilter("LoggingConsoleApp.Program", LogLevel.Debug)
        .AddConsole();
});

//dependency injection setup
var serviceCollection = new ServiceCollection();
serviceCollection.AddTransient<IPipelineProcessor, PipelineProcessor>();
serviceCollection.AddSingleton<IConfiguration>(configuration);
serviceCollection.AddTransient<IDataFetcher, DataFetcher>();
serviceCollection.AddTransient<IMimeMessageBuilder, MimeMessageBuilder>();
serviceCollection.AddTransient<IEmailNotificationSender, EmailNotificationSender>();
serviceCollection.AddScoped<ISmtpClient, SmtpClient>();
serviceCollection.AddTransient<IScraper<AnrScraper>, AnrScraper>();
serviceCollection.AddTransient<IScraper<SabineScraper>, SabineScraper>();

serviceCollection.AddLogging(builder => builder.AddConsole().SetMinimumLevel(LogLevel.Information));

var serviceProvider = serviceCollection.BuildServiceProvider();

#endregion

#region Program
var _appSettings = configuration.Get<AppSettingsModel>()
    ?? throw new ArgumentNullException(nameof(configuration), "AppSettingsModel could not be resolved from the configuration.");
var _pipelineProcessor = serviceProvider.GetService<IPipelineProcessor>()
    ?? throw new InvalidOperationException("PipelineHelper could not be resolved from the service provider.");
var _logger = serviceProvider.GetService<ILogger<Program>>()
    ?? throw new InvalidOperationException("Logger could not be resolved from the service provider.");

try
{    
    if (_appSettings == null || _appSettings.PipelineSettings == null || !_appSettings.PipelineSettings.Any())
    {
        throw new InvalidOperationException("PipelineSettings is not configured correctly in appsettings.json.");
    }

    foreach (var pipelineSetting in _appSettings.PipelineSettings)
    {
        _pipelineProcessor.ProcessPipeline(pipelineSetting.Key);
    }
}
catch (Exception ex)
{
    _logger.LogError($"An error occurred: {ex.Message}");
    throw;
}
#endregion
