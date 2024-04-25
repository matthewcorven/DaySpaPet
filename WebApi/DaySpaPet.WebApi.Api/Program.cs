using System.Reflection;
using Ardalis.ListStartupServices;
using Ardalis.SharedKernel;
using FastEndpoints;
using FastEndpoints.Swagger;
using FastEndpoints.ApiExplorer;
using MediatR;
using Microsoft.EntityFrameworkCore;
using Serilog;
using Serilog.Extensions.Logging;

using DaySpaPet.WebApi.Core;
using DaySpaPet.WebApi.Core.ClientAggregate;
using DaySpaPet.WebApi.Infrastructure;
using DaySpaPet.WebApi.Infrastructure.Data;
using System.Globalization;
using DaySpaPet.WebApi.UseCases;

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

string? connectionString = builder.Configuration.GetConnectionString("SqlServerConnection");
builder.Services.AddDbContext<AppDbContext>(options =>
          options.UseSqlServer(connectionString));

builder.Services.AddFastEndpoints();
//builder.Services.AddFastEndpointsApiExplorer();
builder.Services.SwaggerDocument(o =>
{
  o.ShortSchemaNames = true;
});

//builder.Services.AddSwaggerGen(c =>
//{
//  c.SwaggerDoc("v1", new OpenApiInfo { Title = "My API", Version = "v1" });
//  c.EnableAnnotations();
//  string xmlCommentFilePath = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "swagger-docs.xml");
//  c.IncludeXmlComments(xmlCommentFilePath);
//  c.OperationFilter<FastEndpointsOperationFilter>();
//});

builder.Services.AddCoreServices(microsoftLogger);
builder.Services.AddInfrastructureServices(microsoftLogger, builder.Environment.IsDevelopment());

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

#if DEBUG
EnsureDatabaseCreated(app);
#endif

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

void EnsureDatabaseCreated(WebApplication app)
{
  using (var scope = app.Services.CreateScope())
  {
    var services = scope.ServiceProvider;

#pragma warning disable CA1031 // Do not catch general exception types
    try
    {
      var context = services.GetRequiredService<AppDbContext>();
      //                    context.Database.Migrate();
      var created = context.Database.EnsureCreated();
    }
    catch (Exception ex)
    {
      var logger = services.GetRequiredService<ILogger<Program>>();
      logger.LogError(ex, "An error occurred seeding the DB. {ExceptionMessage}", ex.Message);
    }
#pragma warning restore CA1031 // Do not catch general exception types
  }
}

// Make the implicit Program.cs class public, so integration tests can reference the correct assembly for host building
public partial class Program
{
}
