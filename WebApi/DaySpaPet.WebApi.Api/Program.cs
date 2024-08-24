using Ardalis.GuardClauses;
using Ardalis.ListStartupServices;
using DaySpaPet.WebApi.Api.Endpoints.Behavior;
using DaySpaPet.WebApi.Core;
using DaySpaPet.WebApi.Infrastructure;
using DaySpaPet.WebApi.Infrastructure.Data;
using DaySpaPet.WebApi.SharedKernel;
using DaySpaPet.WebApi.UseCases;
using FastEndpoints;
using FastEndpoints.Security;
using FastEndpoints.Swagger;
using MediatR;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using Serilog;
using Serilog.Sinks.OpenTelemetry;
using Serilog.Templates;
using Serilog.Templates.Themes;
using System.Globalization;
using System.Reflection;
using System.Text;

Log.Logger = GenerateBootstrappedLogger();

try {
  Log.Information("Starting web application");

  WebApplicationBuilder builder = WebApplication.CreateBuilder(args);
  builder.Services.AddSerilog((services, lc) => lc
          // Some additional configuration in addition to that of already boostrapped logger.
          .ReadFrom.Configuration(builder.Configuration)
          .ReadFrom.Services(services));

  builder.Services.AddSharedKernel();
  builder.Services.AddCoreServices();
  // Infrastructure
  builder.Services.AddHttpContextAccessor();
  string? connectionString = builder.Configuration.GetConnectionString("DaySpaPetDb");
  Guard.Against.NullOrEmpty(connectionString, nameof(connectionString));
  builder.Services.AddInfrastructureServices(builder.Environment.IsDevelopment(), connectionString!);

  string? jwtPublicSigningKey = builder.Configuration["Authentication:Schemes:Bearer:PublicSigningKey"];
  string? jwtPrivateSigningKey = builder.Configuration["Authentication:Schemes:Bearer:PrivateSigningKey"];
  string? jwtIssuer = builder.Configuration["Authentication:Schemes:Bearer:ValidIssuer"];
  string? jwtAudiences = builder.Configuration["Authentication:Schemes:Bearer:ValidAudiences"];

  builder.Services
          .AddAuthentication(
               o => {
                 o.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
                 o.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
               });
  builder.Services.AddAuthenticationJwtBearer(
               signingKeyOptions => {
                 // PEM+base64 encoded public-key
                 signingKeyOptions.SigningKey = jwtPublicSigningKey;
                 signingKeyOptions.SigningStyle = TokenSigningStyle.Asymmetric;
                 signingKeyOptions.KeyIsPemEncoded = true;
               },
               jwtBearerOptions => {
                 jwtBearerOptions.TokenValidationParameters.ValidIssuer = jwtIssuer;
                 jwtBearerOptions.TokenValidationParameters.ValidAudience = jwtAudiences;
                 jwtBearerOptions.TokenValidationParameters.ValidateAudience = true;
                 jwtBearerOptions.TokenValidationParameters.ValidateIssuer = true;
                 jwtBearerOptions.TokenValidationParameters.ValidateLifetime = true;
                 jwtBearerOptions.TokenValidationParameters.ValidateIssuerSigningKey = true;
                 jwtBearerOptions.TokenValidationParameters.IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(jwtPrivateSigningKey!));
               });
  builder.Services.AddAuthorization(options => {
    options.AddPolicy("AdministratorsOnly", x => x
            .RequireRole("Administrator")
            .RequireClaim("AdministratorID")
            .RequireClaim("Username")
            .RequireClaim("UserID"));
    options.AddPolicy("ManagersOnly", x => x
            .RequireRole("Manager")
            .RequireClaim("ManagerID")
            .RequireClaim("Username")
            .RequireClaim("UserID"));
    options.AddPolicy("EmployeeID", x => x
            .RequireRole("Employee")
            .RequireClaim("EmployeeID")
            .RequireClaim("UserID"));
  });
  builder.Services.AddFastEndpoints();
  // builder.Services.AddFastEndpointsApiExplorer();
  builder.Services.SwaggerDocument(o => {
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
  ConfigureMediatR(builder);

  // add list services for diagnostic purposes - see https://github.com/ardalis/AspNetCoreStartupServices
  builder.Services.Configure<ServiceConfig>(config => {
    config.Services = new List<ServiceDescriptor>(builder.Services);

    // optional - default path to view services is /listallservices - recommended to choose your own path
    config.Path = "/listservices";
  });


  WebApplication app = builder.Build();

  if (app.Environment.IsDevelopment()) {
    app.UseDeveloperExceptionPage();
    app.UseShowAllServicesMiddleware(); // see https://github.com/ardalis/AspNetCoreStartupServices
  } else {
    app.UseDefaultExceptionHandler(); // from FastEndpoints
    app.UseHsts();
  }

  //Add support to logging request with SERILOG
  app.UseSerilogRequestLogging();

  app.UseAuthentication();
  app.UseAuthorization();
  app.UseFastEndpoints();
  app.UseSwaggerGen(); // FastEndpoints middleware

  app.UseHttpsRedirection();

  // Enable middleware to serve swagger-ui (HTML, JS, CSS, etc.), specifying the Swagger JSON endpoint.
  //app.UseSwaggerUI(c => c.SwaggerEndpoint("/swagger/v1/swagger.json", "My API V1"));

  BootstrapDatabase(builder, app);

  app.Run();
} catch (Exception ex) {
  Log.Fatal(ex, "Application terminated unexpectedly");
} finally {
  Log.CloseAndFlush();
}

// Make the implicit Program.cs class public, so integration tests can reference the correct assembly for host building
public partial class Program {
  static void ConfigureMediatR(WebApplicationBuilder builder) {
    Assembly?[] mediatRAssemblies = new[]
    {
                Assembly.GetAssembly(typeof(CoreAssemblyLocator)),
                Assembly.GetAssembly(typeof(UseCaseAssemblyLocator)),
                Assembly.GetAssembly(typeof(InfrastructureAssemblyLocator))
        };

    builder.Services.AddMediatR(cfg => cfg.RegisterServicesFromAssemblies(mediatRAssemblies!));
    builder.Services.AddScoped(typeof(IPipelineBehavior<,>), typeof(CaptureUserRequestContextBehavior<,>));
    builder.Services.AddScoped(typeof(IPipelineBehavior<,>), typeof(LoggingBehavior<,>));
    builder.Services.AddScoped<IDomainEventDispatcher, MediatRDomainEventDispatcher>();
  }

  static void BootstrapDatabase(WebApplicationBuilder builder, WebApplication app) {
    using IServiceScope scope = app.Services.CreateScope();
    IServiceProvider services = scope.ServiceProvider;

#pragma warning disable CA1031 // Do not catch general exception types
    try {
      AppDbContext context = services.GetRequiredService<AppDbContext>();
      IConfigurationSection dbConfig = builder.Configuration.GetSection("Infrastructure:Databases:DaySpaPetDb");

      bool? dbWasDropped = null;
      if (dbConfig.HasTruthySectionValue("Drop", out string configuredToDrop)) {
        Log.Logger.Warning("Dropping database because the environment variable \"Drop\" has truthy value: {ConfiguredToDrop}", configuredToDrop);
        dbWasDropped = context.Database.EnsureDeleted();
      }
      if (dbConfig.HasTruthySectionValue("MustExist", out string configuredToCreateDb)) {
        Log.Logger.Information("Ensuring database exists because the environment variable \"MustExist\" has truthy value: {configuredToCreateDb}", configuredToCreateDb);
        context.Database.EnsureCreated();

        if (dbConfig.HasTruthySectionValue("RunAnyNewMigrations", out string configuredToRunNewMigrations)) {
          Log.Logger.Information($"Initatiing any newly-added database migrations because the environment variable \"RunAnyNewMigrations\" has truthy value");
          context.Database.Migrate();
        }
      } else {
        if (dbWasDropped!.Value) {
          Log.Logger.Information($"Exiting early: Database does not exist, and also the environment variable \"MustExist\" has falsy value so no database will be created at this time.");
          Environment.Exit(0);
        }
      }

    } catch (Exception ex) {
      Log.Logger.Fatal(ex, "An error occurred seeding the DB. {ExceptionMessage}", ex.Message);
    }
#pragma warning restore CA1031 // Do not catch general exception types
  }

  static Serilog.Extensions.Hosting.ReloadableLogger GenerateBootstrappedLogger() {
    Serilog.Extensions.Hosting.ReloadableLogger logger = new LoggerConfiguration()
#if DEBUG
            .MinimumLevel.Debug()
            .WriteTo.Console(new ExpressionTemplate(
            // Include trace and span ids when present.
            "[{@t:HH:mm:ss} {@l:u3}{#if @tr is not null} ({substring(@tr,0,4)}:{substring(@sp,0,4)}){#end}] {@m}\n{@x}",
            theme: TemplateTheme.Code))
            .WriteTo.File(
                    path: "logs/log-debug.txt",
                    rollingInterval: RollingInterval.Hour,
                    rollOnFileSizeLimit: true, formatter: new ExpressionTemplate(
                            // Include trace and span ids when present.
                            "[{@t:HH:mm:ss} {@l:u3}{#if @tr is not null} ({substring(@tr,0,4)}:{substring(@sp,0,4)}){#end}] {@m}\n{@x}",
                            theme: TemplateTheme.Code, formatProvider: new CultureInfo("en-US")
                    )
            )
#else
						.MinimumLevel.Information()
#endif
            // https://github.com/serilog/serilog-sinks-opentelemetry
            .WriteTo.OpenTelemetry(options => {
              options.Endpoint = "http://localhost:4317/v1/logs";
              options.Protocol = OtlpProtocol.Grpc;
              options.ResourceAttributes = new Dictionary<string, object> {
                ["service.name"] = "dayspa-pet--web-api--api"
              };
            })
            .CreateBootstrapLogger();
    return logger;
  }
}