using Ardalis.GuardClauses;
using Ardalis.ListStartupServices;
using MediatR;
using Microsoft.EntityFrameworkCore;
using FastEndpoints;
using FastEndpoints.Swagger;
using Serilog;
using Serilog.Extensions.Logging;
using System.Globalization;
using System.Reflection;

using DaySpaPet.WebApi.Core;
using DaySpaPet.WebApi.Infrastructure;
using DaySpaPet.WebApi.Infrastructure.Data;
using DaySpaPet.WebApi.UseCases;
using DaySpaPet.WebApi.SharedKernel;
using FastEndpoints.ApiExplorer;

var logger = Log.Logger = new LoggerConfiguration()
  .Enrich.FromLogContext()
  .WriteTo.Console(formatProvider: new CultureInfo("en-US"))
  .CreateLogger();

logger.Information("Starting web host");

var builder = WebApplication.CreateBuilder(args);

builder.Host.UseSerilog((_, config) => config.ReadFrom.Configuration(builder.Configuration));
using var loggerFactory = new SerilogLoggerFactory(logger);
var microsoftLogger = loggerFactory.CreateLogger<Program>();

builder.Services.Configure<CookiePolicyOptions>(options =>
{
  options.CheckConsentNeeded = context => true;
  options.MinimumSameSitePolicy = SameSiteMode.None;
});

builder.Services.AddSharedKernel(microsoftLogger);
builder.Services.AddCoreServices(microsoftLogger);
// Infrastructure
builder.Services.AddHttpContextAccessor();
string? connectionString = builder.Configuration.GetConnectionString("DaySpaPetDb");
Guard.Against.NullOrEmpty(connectionString, nameof(connectionString));
builder.Services.AddInfrastructureServices(microsoftLogger, builder.Environment.IsDevelopment(), connectionString!);


builder.Services.AddFastEndpoints();
// builder.Services.AddFastEndpointsApiExplorer();
builder.Services.SwaggerDocument(o =>
{
  o.ShortSchemaNames = true;
  o.DocumentSettings = s => s.OperationProcessors.Add(new AddRequestOriginClockTimeZoneId(_ => _.Strict = true));
});

//builder.Services.AddSwaggerGen(c =>
//{
//  c.SwaggerDoc("v1", new OpenApiInfo { Title = "My API", Version = "v1" });
//  c.EnableAnnotations();
//  string xmlCommentFilePath = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "swagger-docs.xml");
//  c.IncludeXmlComments(xmlCommentFilePath);
//  c.OperationFilter<FastEndpointsOperationFilter>();
//});
ConfigureMediatR();

// add list services for diagnostic purposes - see https://github.com/ardalis/AspNetCoreStartupServices
builder.Services.Configure<ServiceConfig>(config =>
{
  config.Services = new List<ServiceDescriptor>(builder.Services);

  // optional - default path to view services is /listallservices - recommended to choose your own path
  config.Path = "/listservices";
});


var app = builder.Build();

if (app.Environment.IsDevelopment())
{
  app.UseDeveloperExceptionPage();
  app.UseShowAllServicesMiddleware(); // see https://github.com/ardalis/AspNetCoreStartupServices
}
else
{
  app.UseDefaultExceptionHandler(); // from FastEndpoints
  app.UseHsts();
}
app.UseFastEndpoints();
app.UseSwaggerGen(); // FastEndpoints middleware

app.UseHttpsRedirection();

// Enable middleware to serve swagger-ui (HTML, JS, CSS, etc.), specifying the Swagger JSON endpoint.
//app.UseSwaggerUI(c => c.SwaggerEndpoint("/swagger/v1/swagger.json", "My API V1"));

BootstrapDatabase(app);

app.Run();

void ConfigureMediatR()
{
  var mediatRAssemblies = new[]
  {
    Assembly.GetAssembly(typeof(CoreAssemblyLocator)),
    Assembly.GetAssembly(typeof(UseCaseAssemblyLocator)),
    Assembly.GetAssembly(typeof(InfrastructureAssemblyLocator))
  };
  
  builder.Services.AddMediatR(cfg => cfg.RegisterServicesFromAssemblies(mediatRAssemblies!));
  builder.Services.AddScoped(typeof(IPipelineBehavior<,>), typeof(LoggingBehavior<,>));
  builder.Services.AddScoped<IDomainEventDispatcher, MediatRDomainEventDispatcher>();
}

void BootstrapDatabase(WebApplication app)
{
  using var scope = app.Services.CreateScope();
  var services = scope.ServiceProvider;

#pragma warning disable CA1031 // Do not catch general exception types
  try
  {
    var context = services.GetRequiredService<AppDbContext>();
    var loggerFactory = new LoggerFactory();
    var logger = loggerFactory.CreateLogger(app.GetType());

    var dbConfig = builder.Configuration.GetSection("Infrastructure:Databases:DaySpaPetDb");

    bool? dbWasDropped = null;
    if (dbConfig.HasTruthySectionValue("Drop"))
    {
      logger.LogWarning("Dropping database because the environment variable \"Drop\" has truthy value");
      dbWasDropped = context.Database.EnsureDeleted();
    }
    if (dbConfig.HasTruthySectionValue("MustExist"))
    {
      logger.LogInformation($"Ensuring database exists because the environment variable \"MustExist\" has truthy value");
      context.Database.EnsureCreated();

      if (dbConfig.HasTruthySectionValue("RunAnyPendingMigrations"))
      {
        logger.LogInformation($"Initatiing any pending database migrations because the environment variable \"RunAnyPendingMigrations\" has truthy value");
        context.Database.Migrate();
      }
    }
    else
    {
      if (dbWasDropped!.Value)
      {
        logger.LogInformation($"Exiting early: Database does not exist, and also the environment variable \"MustExist\" has falsy value");
        Environment.Exit(0);
      }
    }

  }
  catch (Exception ex)
  {
    var logger = services.GetRequiredService<ILogger<Program>>();
    logger.LogError(ex, "An error occurred seeding the DB. {ExceptionMessage}", ex.Message);
  }
#pragma warning restore CA1031 // Do not catch general exception types
}

// Make the implicit Program.cs class public, so integration tests can reference the correct assembly for host building
public partial class Program
{
}
