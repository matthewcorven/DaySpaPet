using Aspire.Hosting;
using Microsoft.Extensions.Configuration;

IDistributedApplicationBuilder builder = DistributedApplication.CreateBuilder(args);

//IResourceBuilder<ContainerResource> _ = builder.AddDockerfile("dayspapet-app-container", "..");

IResourceBuilder<SqlServerServerResource> sql = builder.AddSqlServer("dayspapet-sql-server");
IResourceBuilder<SqlServerDatabaseResource> sqlDb = sql.AddDatabase("DaySpaPetDb");

//var connectionString = builder.AddConnectionString("DaySpaPetDb");

builder.AddProject<Projects.DaySpaPet_WebApi_Api>("dayspapet-webapi-api")
  //.WithEnvironment("ConnectionStrings__DaySpaPetDb", builder.Configuration.GetConnectionString("DaySpaPetDb"))
  .WithReference(sql)
  .WithEnvironment("Authentication__Schemes__Bearer__PrivateSigningKey", builder.Configuration.GetValue<string>("Authentication__Schemes__Bearer__PrivateSigningKey"))
  .WithEnvironment("Authentication__Schemes__Bearer__PublicSigningKey", builder.Configuration.GetValue<string>("Authentication__Schemes__Bearer__PublicSigningKey"));

builder.Build().Run();
