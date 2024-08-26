var builder = DistributedApplication.CreateBuilder(args);

builder.AddProject<Projects.DaySpaPet_WebApi_Api>("dayspapet-webapi-api");

builder.Build().Run();
